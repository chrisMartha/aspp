using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PSoC.ManagementService.Data.Interfaces
{
    /// <summary>
    /// Interface for data repository
    /// </summary>
    /// <typeparam name="TEntity">Data model entity type</typeparam>
    /// <typeparam name="TKey">Type of primary key</typeparam>
    public interface IDataRepository<TEntity, in TKey>
        where TEntity : class
    {
        /// <summary>
        /// Delete data model entity object from the database matching primary key value
        /// </summary>
        /// <param name="key">Primary key value</param>
        /// <returns>True if data model entity was deleted successfully, false otherwise</returns>
        Task<bool> DeleteAsync(TKey key);

        /// <summary>
        /// Delete multiple data model entity object from the database matching primary key values
        /// </summary>
        /// <param name="keys">Primary key value</param>
        /// <returns>True if data models entity were deleted successfully, false otherwise</returns>
        Task<bool> DeleteAsync(TKey[] keys);

        /// <summary>
        /// Retrieve and return all data model entities
        /// </summary>
        /// <returns>List of all data model entities</returns>
        Task<IList<TEntity>> GetAsync();

        /// <summary>
        /// Retrieve and return data model entity object matching primary key value
        /// </summary>
        /// <param name="key">Primary key value</param>
        /// <returns>Data model entity object</returns>
        Task<TEntity> GetByIdAsync(TKey key);

        /// <summary>
        /// Insert new data model entity object into the database
        /// </summary>
        /// <param name="entity">Pre-update data model entity object</param>
        /// <returns>Updated data model entity object</returns>
        Task<TEntity> InsertAsync(TEntity entity);

        /// <summary>
        /// Update existing data model entity object in the database
        /// </summary>
        /// <param name="entity">Data model entity object</param>
        /// <returns>Data model entity object</returns>
        Task<TEntity> UpdateAsync(TEntity entity);
    }
}
