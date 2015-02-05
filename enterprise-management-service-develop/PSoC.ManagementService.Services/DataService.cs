using System.Collections.Generic;
using System.Threading.Tasks;
using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Services.Interfaces;

namespace PSoC.ManagementService.Services
{
    /// <summary>
    /// Base class for data services
    /// </summary>
    /// <typeparam name="TEntity">Type of entity model</typeparam>
    /// <typeparam name="TKey">Type of primary key</typeparam>
    public abstract class DataService<TEntity, TKey> : IDataService<TEntity, TKey> where TEntity : class
    {
        protected IUnitOfWork UnitOfWork { get; private set; }

        protected DataService(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        /// <summary>
        /// Retrieve all items from the database
        /// </summary>
        /// <returns>List of database model objects</returns>
        public async Task<IList<TEntity>> GetAsync()
        {
            IDataRepository<TEntity, TKey> dataRepository = UnitOfWork.GetDataRepository<TEntity, TKey>();
            IList<TEntity> resultList = await dataRepository.GetAsync().ConfigureAwait(false);

            return resultList;
        }

        /// <summary>
        /// Retrieve an item from the database
        /// </summary>
        /// <param name="key">Unique database item identifier, i.e. value of primary key</param>
        /// <returns>A database model object</returns>
        public async Task<TEntity> GetByIdAsync(TKey key)
        {
            IDataRepository<TEntity, TKey> dataRepository = UnitOfWork.GetDataRepository<TEntity, TKey>();
            TEntity result = await dataRepository.GetByIdAsync(key).ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// Add new item to the database
        /// </summary>
        /// <param name="entity">New database model object</param>
        /// <returns>Updated database model object, e.g. with identity primary key populated</returns>
        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            IDataRepository<TEntity, TKey> dataRepository = UnitOfWork.GetDataRepository<TEntity, TKey>();
            TEntity result = await dataRepository.InsertAsync(entity).ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// Update existing item in the database
        /// </summary>
        /// <param name="entity">Database model object</param>
        /// <returns>Database model object</returns>
        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            IDataRepository<TEntity, TKey> dataRepository = UnitOfWork.GetDataRepository<TEntity, TKey>();
            TEntity result = await dataRepository.UpdateAsync(entity).ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// Delete existing item from the database
        /// </summary>
        /// <param name="key">Unique database item identifier, i.e. value of primary key</param>
        /// <returns>True if item was deleted successfully, false otherwise</returns>
        public async Task<bool> DeleteAsync(TKey key)
        {
            IDataRepository<TEntity, TKey> dataRepository = UnitOfWork.GetDataRepository<TEntity, TKey>();
            bool result = await dataRepository.DeleteAsync(key).ConfigureAwait(false);

            return result;
        }
    }
}
