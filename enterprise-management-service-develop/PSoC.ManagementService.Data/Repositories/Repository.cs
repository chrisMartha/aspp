using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using PSoC.ManagementService.Data.Helpers;
using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Data.Types;

namespace PSoC.ManagementService.Data.Repositories
{
    public abstract class Repository<TEntity, TQueryFactory, TDataMapper, TKey> : IDataRepository<TEntity, TKey>
        where TEntity : class, IEntity, new()
        where TQueryFactory : IQueryFactory<TEntity, TKey>, new()
        where TDataMapper : IDataMapper<TEntity>, new()
    {
        private readonly IQueryFactory<TEntity, TKey> _factory;
        private readonly IDataMapper<TEntity> _mapper;
        private PropertyInfo _primaryKeyPropertyInfo;

        #region Constructors

        protected Repository()
        {
            _factory = new TQueryFactory();
            _mapper = new TDataMapper();
        }

        #endregion Constructors

        public async Task<bool> DeleteAsync(TKey key)
        {
            var query = GetDeleteQuery(key);
            var result = await ExecuteQueryAsync(query.QueryString, query.SqlParameters).ConfigureAwait(false);

            return result;
        }

        public async Task<bool> DeleteAsync(TKey[] keys)
        {
            var query = GetDeleteManyQuery(keys);
            var result = await ExecuteQueryAsync(query.QueryString, query.SqlParameters).ConfigureAwait(false);

            return result;
        }

        public async Task<IList<TEntity>> GetAsync()
        {
            var query = GetSelectQuery();
            var result = await GetQueryResultAsync(query.QueryString).ConfigureAwait(false);

            return result;
        }

        public async Task<TEntity> GetByIdAsync(TKey key)
        {
            var keyProp = GetKeyProperty();
            string varName = "@" + keyProp.Name;

            var paramList = new List<SqlParameter>
            {
                new SqlParameter(varName, keyProp.PropertyType.ToSqlDbType()) { Value = key}
            };

            var query = GetSelectQuery(string.Format("WHERE {0} = {1}", keyProp.Name, varName), loadNestedTypes: true);
            var result = await GetQueryResultAsync(query.QueryString, paramList, loadNestedTypes: true).ConfigureAwait(false);

            return result.FirstOrDefault();
        }

        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            var query = GetInsertQuery(entity);
            query.QueryString = "BEGIN TRANSACTION" + Environment.NewLine + query.QueryString
                 + Environment.NewLine + "COMMIT TRANSACTION";
            return await InsertUpdateAsync(query).ConfigureAwait(false);
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            var query = GetUpdateQuery(entity);

            query.QueryString = "BEGIN TRANSACTION" + Environment.NewLine + query.QueryString
                 + Environment.NewLine + "COMMIT TRANSACTION";

            return await InsertUpdateAsync(query).ConfigureAwait(false);
        }

        protected async Task<bool> ExecuteQueryAsync(string query, ICollection<SqlParameter> paramList = null)
        {
            var result = await DataAccessHelper.ExecuteAsync(query, paramList).ConfigureAwait(false);
                bool success = (result >= 1);

                return success;
        }

        protected QueryObject GetDeleteManyQuery(TKey[] keys)
        {
            return _factory.GetDeleteManyQuery(keys);
        }

        protected QueryObject GetDeleteQuery(TKey key)
        {
            return _factory.GetDeleteQuery(key);
        }

        protected QueryObject GetInsertQuery(TEntity entity)
        {
            return _factory.GetInsertQuery(entity);
        }

        /// <summary>
        /// Gets the result of the SQL query
        /// </summary>
        /// <param name="query">the SQL Query</param>
        /// <param name="paramList">list of parameters if any.</param>
        /// <param name="loadNestedTypes">load related data from other tables.</param>
        /// <returns></returns>
        protected async Task<IList<TEntity>> GetQueryResultAsync(string query, ICollection<SqlParameter> paramList = null, bool loadNestedTypes = false)
        {
            IList<TEntity> result;

            using (SqlDataReader dr = await DataAccessHelper.GetDataReaderAsync(query, paramList).ConfigureAwait(false))
            {
                result = await _mapper.ToEntityListAsync(dr, loadNestedTypes).ConfigureAwait(false);
            }

            return result;
        }

        protected QueryObject GetSelectQuery(string whereClause = "", ICollection<SqlParameter> parameters = null, bool loadNestedTypes = false)
        {
            return _factory.GetSelectQuery(whereClause, parameters, loadNestedTypes);
        }

        protected QueryObject GetUpdateQuery(TEntity entity)
        {
            return _factory.GetUpdateQuery(entity);
        }

        public PropertyInfo GetKeyProperty()
        {
            if (_primaryKeyPropertyInfo == null)
            {
                // Start with type of database object itself, e.g.
                // Example 1: UserDto (for UserDto class)
                // Example 2: AdminDto (for AdminDto class)
                Type primaryKeyType = typeof(TEntity);

                do
                {
                    // For the given type, find all public properties that are marked with [Key] attribute
                    IEnumerable<PropertyInfo> keys = primaryKeyType
                        .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .Where(p => p.IsDefined(typeof(KeyAttribute), false));

                    // Take the first one of them:
                    // Example 1: UserID (for UserDto class)
                    // Example 2, Iteration 1: User (for AdminDto class)
                    // Example 2, Iteration 2: UserID (for AdminDto class)
                    _primaryKeyPropertyInfo = keys.First();

                    // ...its type will be:
                    // Example 1: Guid (for UserDto class)
                    // Example 2, Iteration 1: UserDto (for AdminDto class)
                    // Example 2, Iteration 2: Guid (for AdminDto class)
                    primaryKeyType = _primaryKeyPropertyInfo.PropertyType;

                    // Repeat the loop as long as our primary key type is not a database object type
                    // Example 1: Loop runs once
                    // Example 2: Loop runs two times
                } while (typeof(IEntity).IsAssignableFrom(primaryKeyType));
            }

            return _primaryKeyPropertyInfo;
        }

        private async Task<TEntity> InsertUpdateAsync(QueryObject query)
        {
            var keyProp = GetKeyProperty();

            query.QueryString += Environment.NewLine
                + GetSelectQuery(string.Format("WHERE {0} = @{0}", keyProp.Name), loadNestedTypes: true).QueryString;

            var result = await GetQueryResultAsync(query.QueryString, query.SqlParameters, true).ConfigureAwait(false);

            return result.FirstOrDefault();
        }

    }
}