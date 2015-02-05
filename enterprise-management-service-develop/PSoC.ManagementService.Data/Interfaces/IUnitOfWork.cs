namespace PSoC.ManagementService.Data.Interfaces
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// Access point repository
        /// </summary>
        IAccessPointRepository AccessPointRepository
        {
            get;
        }

        /// <summary>
        /// Admin repository
        /// </summary>
        IAdminRepository AdminRepository
        {
            get;
        }

        /// <summary>
        /// District repository
        /// </summary>
        IDistrictRepository DistrictRepository
        {
            get;
        }

        /// <summary>
        /// District repository
        /// </summary>
        ISchoolRepository SchoolRepository
        {
            get;
        }

        /// <summary>
        /// Get data repositority (in a generic way) for a given data model entity and primary key types
        /// </summary>
        /// <typeparam name="TEntity">Type of data model entity</typeparam>
        /// <typeparam name="TKey">Type of primary key</typeparam>
        /// <returns>Data repository</returns>
        IDataRepository<TEntity, TKey> GetDataRepository<TEntity, TKey>()
            where TEntity : class;
    }
}