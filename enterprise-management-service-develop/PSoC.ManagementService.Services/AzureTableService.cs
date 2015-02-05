using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using PSoC.ManagementService.Services.Interfaces;
using PSoC.ManagementService.Services.Logging;

namespace PSoC.ManagementService.Services
{
    public class AzureTableService : IAzureTableService
    {
        private readonly IConfigBase _configBase;
        private readonly CloudStorageAccount _storageAccount;
        private readonly CloudTableClient _tableClient;

        public AzureTableService(string azureConnectionString)
        {
            _storageAccount = CloudStorageAccount.Parse(azureConnectionString);
            _tableClient = _storageAccount.CreateCloudTableClient();
        }
        
        [InjectionConstructor]
        public AzureTableService(IConfigBase configBase)
        {
            _configBase = configBase;
            _storageAccount = CloudStorageAccount.Parse(configBase.GetApplicationSetting<string>("StorageConnectionString"));
            _tableClient = _storageAccount.CreateCloudTableClient();
        }

        /// <summary>
        /// Common method to get a given container in the azure account
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        private async Task<CloudTable> GetTableAsync(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                var ex = new ArgumentNullException("tableName");
                PEMSEventSource.Log.AzureTableServiceException(ex.Message);
                throw ex;
            }

            var table = _tableClient.GetTableReference(tableName);
            if(table == null)
            {                
                PEMSEventSource.Log.AzureTableServiceInvalidTable(_storageAccount.TableEndpoint.AbsoluteUri, tableName);
                throw new Exception("Table does not exist");
            }

            if (!table.Exists())
            {
                await table.CreateIfNotExistsAsync();
            }

            return table;
        }

        public async Task<bool> InsertEntityAsync<T>(string tableName, T entity) where T : TableEntity, new()
        {
            if (entity == null)
            {
                var ex = new ArgumentNullException("Entity was null");
                PEMSEventSource.Log.AzureTableServiceException(ex.Message);
                throw ex;
            }

            var table = await GetTableAsync(tableName);
            try
            {
                var insertOperation = TableOperation.Insert(entity);
                await table.ExecuteAsync(insertOperation);
                return true;
            }
            catch (Exception ex)
            {
                PEMSEventSource.Log.AzureTableServiceInsertException(ex.Message);
                throw;
            }
        }

        public async Task<bool> InsertOrReplaceEntityAsync<T>(string tableName, T entity) where T : TableEntity, new()
        {

            if (entity == null)
            {
                var ex = new ArgumentNullException("Entity was null");
                PEMSEventSource.Log.AzureTableServiceException(ex.Message);
                throw ex;
            }

            var table = await GetTableAsync(tableName);
            try
            {
                var insertOperation = TableOperation.InsertOrReplace(entity);
                await table.ExecuteAsync(insertOperation);
                return true;
            }
            catch (Exception ex)
            {
                PEMSEventSource.Log.AzureTableServiceInsertException(ex.Message);
                throw;
            }
        }

        public async Task<bool> InsertBatchEntitiesAsync<T>(string tableName, List<T> entities) where T : TableEntity, new()
        {
            if (entities == null)
            {
                var ex = new ArgumentNullException("Entity list was null");
                PEMSEventSource.Log.AzureTableServiceException(ex.Message);
                throw ex;
            }

            var table = await GetTableAsync(tableName);
            try
            {
                var batchOperation = new TableBatchOperation();
                foreach (var entity in entities)
                {
                    if (entity != null)
                    {
                        batchOperation.Insert(entity);
                    }
                }

                await table.ExecuteBatchAsync(batchOperation);
                return true;
            }
            catch (Exception ex)
            {
                PEMSEventSource.Log.AzureTableServiceInsertException(ex.Message);
                throw;
            }
        }

        public async Task<bool> InsertOrReplaceBatchEntitiesAsync<T>(string tableName, List<T> entities) where T : TableEntity, new()
        {
            if (entities == null)
            {
                var ex = new ArgumentNullException("Entity list was null");
                PEMSEventSource.Log.AzureTableServiceException(ex.Message);
                throw ex;
            }

            var table = await GetTableAsync(tableName);
            try
            {
                var batchOperation = new TableBatchOperation();
                foreach (var entity in entities)
                {
                    if (entity != null)
                    {
                        batchOperation.InsertOrReplace(entity);
                    }
                }

                await table.ExecuteBatchAsync(batchOperation);
                return true;
            }
            catch (Exception ex)
            {
                PEMSEventSource.Log.AzureTableServiceInsertException(ex.Message);
                throw;
            }
        }

        public async Task<T> GetEntityByPartitionRowKeyAsync<T>(string tableName, string partitionKey, string rowKey) where T : TableEntity, new()
        {
            if (string.IsNullOrEmpty(partitionKey) || string.IsNullOrEmpty(rowKey))
            {
                var ex = new ArgumentNullException("Null partition or row key");
                PEMSEventSource.Log.AzureTableServiceException(ex.Message);
                throw ex;
            }

            var table = await GetTableAsync(tableName);

            try
            {
                var retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
                var retrievedResult = await table.ExecuteAsync(retrieveOperation);
                return (T)retrievedResult.Result;
            }
            catch (Exception ex)
            {
                PEMSEventSource.Log.AzureTableServiceGetException(ex.Message);
                throw;
            }
        }

        public async Task<IQueryable<T>> GetEntitiesByPartitionKeys<T>(string tableName, List<string> partitionKeys) where T : TableEntity, new()
        {
            if (partitionKeys == null)
            {
                var ex = new ArgumentNullException("Null partition keys list");
                PEMSEventSource.Log.AzureTableServiceException(ex.Message);
                throw ex;
            }

            if(partitionKeys.Count == 0)
            {
                return Enumerable.Empty<T>().AsQueryable();
            }

            var table = await GetTableAsync(tableName);
            try
            {
                var query = table.CreateQuery<T>();

                if (partitionKeys.Count == 1)
                {
                    return query.Where(x => x.PartitionKey == partitionKeys[0]);
                }

                //Build a dynamic where clause of all partition keys.
                var pe = Expression.Parameter(typeof(T), "d");
                var partitionPropertyExpression = Expression.Property(pe, "PartitionKey");

                Expression left = null;
                foreach (var key in partitionKeys)
                {
                    var rightTemp = Expression.Constant(key);
                    var expTemp = Expression.Equal(partitionPropertyExpression, rightTemp);
                    if (left != null)
                    {
                        left = Expression.OrElse(left, expTemp);
                    }
                    else
                    {
                        left = expTemp;
                    }
                }

                var whereCallExpression = Expression.Call(
                    typeof(Queryable),
                    "Where",
                    new Type[] { query.ElementType },
                    query.Expression,
                    Expression.Lambda<Func<T, bool>>(left, new ParameterExpression[] { pe }));

                var results = query.Provider.CreateQuery<T>(whereCallExpression);

                return results;
            }
            catch (Exception ex)
            {
                PEMSEventSource.Log.AzureTableServiceGetException(ex.Message);
                throw;
            }
        }

        public async Task<IQueryable<T>> GetEntitiesByPartitionKey<T>(string tableName, string partitionKey) where T : TableEntity, new()
        {
            if (string.IsNullOrEmpty(partitionKey))
            {
                var ex = new ArgumentNullException("Null partition key");
                PEMSEventSource.Log.AzureTableServiceException(ex.Message);
                throw ex;
            }

            var table = await GetTableAsync(tableName);
            try
            {
                var result = (from d in table.CreateQuery<T>()
                              where d.PartitionKey == partitionKey
                              select d);

                return result;
            }
            catch (Exception ex)
            {
                PEMSEventSource.Log.AzureTableServiceGetException(ex.Message);
                throw;
            }
        }

        public async Task<IQueryable<T>> GetEntitiesByRowKey<T>(string tableName, string rowKey) where T : TableEntity, new()
        {
            if (string.IsNullOrEmpty(rowKey))
            {
                var ex = new ArgumentNullException("Null row key");
                PEMSEventSource.Log.AzureTableServiceException(ex.Message);
                throw ex;
            }

            var table = await GetTableAsync(tableName);
            try
            {
                var result = (from d in table.CreateQuery<T>()
                               where d.RowKey == rowKey
                               select d);

                return result;
            }
            catch (Exception ex)
            {
                PEMSEventSource.Log.AzureTableServiceGetException(ex.Message);
                throw;
            }
        }

        public async Task<IQueryable<T>> GetAllEntities<T>(string tableName) where T : TableEntity, new()
        {
            var table = await GetTableAsync(tableName);
            try
            {
                var result = table.CreateQuery<T>();
                return result;
            }
            catch (Exception ex)
            {
                PEMSEventSource.Log.AzureTableServiceGetException(ex.Message);
                throw;
            }
        }

        public async Task<bool> DeleteEntityAsync<T>(string tableName, T entity) where T : TableEntity, new()
        {
            if (entity == null)
            {
                var ex = new ArgumentNullException("Entity was null");
                PEMSEventSource.Log.AzureTableServiceException(ex.Message);
                throw ex;
            }

            var table = await GetTableAsync(tableName);
            try
            {
                var deleteOperation = TableOperation.Delete(entity);
                await table.ExecuteAsync(deleteOperation);
                return true;
            }
            catch (Exception ex)
            {
                PEMSEventSource.Log.AzureTableServiceDeleteException(ex.Message);
                throw;
            }
        }

        public async Task<bool> DeleteBatchEntitiesAsync<T>(string tableName, List<T> entities) where T : TableEntity, new()
        {
            if (entities == null)
            {
                var ex = new ArgumentNullException("Entities list was null");
                PEMSEventSource.Log.AzureTableServiceException(ex.Message);
                throw ex;
            }

            var table = await GetTableAsync(tableName);
            try
            {
                var batchOperation = new TableBatchOperation();
                foreach (var entity in entities)
                {
                    if (entity != null)
                    {
                        batchOperation.Delete(entity);
                    }
                }

                await table.ExecuteBatchAsync(batchOperation);
                return true;
            }
            catch (Exception ex)
            {
                PEMSEventSource.Log.AzureTableServiceDeleteException(ex.Message);
                throw;
            }
        }
    }
}
