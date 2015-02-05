using System.Collections.Generic;
using System.Threading.Tasks;

namespace PSoC.ManagementService.Services.Interfaces
{
    /// <summary>
    /// Interface for base data service class
    /// </summary>
    /// <typeparam name="TEntity">Type of entity model</typeparam>
    /// <typeparam name="TKey">Type of primary key</typeparam>
    public interface IDataService<TEntity, in TKey>
    {
        /// <summary>
        /// Retrieve all items from the database
        /// </summary>
        /// <returns>List of database model objects</returns>
        Task<IList<TEntity>> GetAsync();

        /// <summary>
        /// Retrieve an item from the database
        /// </summary>
        /// <returns></returns>
        Task<TEntity> GetByIdAsync(TKey key);

        /// <summary>
        /// Add new item to the database
        /// </summary>
        /// <param name="entity">New database model object</param>
        /// <returns>Updated database model object, e.g. with identity primary key populated</returns>
        Task<TEntity> CreateAsync(TEntity entity);

        /// <summary>
        /// Update existing item in the database
        /// </summary>
        /// <param name="entity">Database model object</param>
        /// <returns>Database model object</returns>
        Task<TEntity> UpdateAsync(TEntity entity);

        /// <summary>
        /// Delete existing item from the database
        /// </summary>
        /// <param name="key">Unique database item identifier, i.e. value of primary key</param>
        /// <returns>True if item was deleted successfully, false otherwise</returns>
        Task<bool> DeleteAsync(TKey key);
    }
}
