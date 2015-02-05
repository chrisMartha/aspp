using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace PSoC.ManagementService.Services.Interfaces
{
    public interface IAzureTableService
    {
        Task<bool> InsertEntityAsync<T>(string tableName, T entity) where T : TableEntity, new();
        Task<bool> InsertOrReplaceEntityAsync<T>(string tableName, T entity) where T : TableEntity, new();
        Task<bool> InsertBatchEntitiesAsync<T>(string tableName, List<T> entities) where T : TableEntity, new();
        Task<bool> InsertOrReplaceBatchEntitiesAsync<T>(string tableName, List<T> entities) where T : TableEntity, new();
        Task<T> GetEntityByPartitionRowKeyAsync<T>(string tableName, string partitionKey, string rowKey) where T : TableEntity, new();
        Task<IQueryable<T>> GetEntitiesByPartitionKeys<T>(string tableName, List<string> partitionKey) where T : TableEntity, new();     
        Task<IQueryable<T>> GetEntitiesByPartitionKey<T>(string tableName, string partitionKey) where T : TableEntity, new();
        Task<IQueryable<T>> GetEntitiesByRowKey<T>(string tableName, string rowKey) where T : TableEntity, new();
        Task<IQueryable<T>> GetAllEntities<T>(string tableName) where T : TableEntity, new();
        Task<bool> DeleteEntityAsync<T>(string tableName, T entity) where T : TableEntity, new();
        Task<bool> DeleteBatchEntitiesAsync<T>(string tableName, List<T> entities) where T : TableEntity, new();
    }
}
