using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PSoC.ManagementService.Core;
using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Services.Interfaces;
using PSoC.ManagementService.Services.Logging;
using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.Services
{
    /// <summary>
    /// District service
    /// </summary>
    public class DistrictService : IDistrictService
    {
        protected IUnitOfWork UnitOfWork { get; private set; }

        public DistrictService(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        /// <summary>
        /// Retrieve all items from the database
        /// </summary>
        /// <returns>List of database model objects</returns>
        public async Task<IList<District>> GetAsync()
        {
            try
            {
                var resultList = new List<District>();

                var dataRepository = UnitOfWork.GetDataRepository<DistrictDto, Guid>();
                foreach (var item in await dataRepository.GetAsync().ConfigureAwait(false))
                {
                    resultList.Add((District)item);
                }

                return resultList;
            }
            catch (Exception ex)
            {
                PEMSEventSource.Log.DistrictServiceException(ex.Message, logRequest: new LogRequest { Exception = ex });
                throw;
            }
        }

        /// <summary>
        /// Retrieve all items from the database with access check
        /// </summary>
        /// <param name="username">User name</param>
        /// <returns>List of database model objects</returns>
        public async Task<IList<District>> GetAsync(String username)
        {
            AdminDto admin = null;
            try
            {
                var resultList = new List<District>();

                // Access check
                admin = await UnitOfWork.AdminRepository.GetByUsernameAsync(username).ConfigureAwait(false);
                if (!IsAuthorized(admin))
                    throw new UnauthorizedAccessException("Unauthorized to retrieve districts.");

                switch (admin.AdminType)
                {
                    case AdminType.GlobalAdmin:
                        resultList.AddRange(await GetAsync().ConfigureAwait(false));
                        break;
                    case AdminType.DistrictAdmin:
                        var district = await GetByIdAsync(admin.District.DistrictId).ConfigureAwait(false);
                        if (district != null)
                            resultList.Add(district);
                        break;
                }

                return resultList;
            }
            catch (Exception ex)
            {
                var userId = (admin != null) ? admin.User.UserID.ToString() : null;
                PEMSEventSource.Log.DistrictServiceException(ex.Message, userId: userId, logRequest: new LogRequest { Exception = ex });
                throw;
            }
        }

        /// <summary>
        /// Retrieve an item from the database
        /// </summary>
        /// <param name="key">Unique database item identifier, i.e. value of primary key</param>
        /// <returns>A database model object</returns>
        public async Task<District> GetByIdAsync(Guid key)
        {
            try
            {
                var dataRepository = UnitOfWork.GetDataRepository<DistrictDto, Guid>();
                return (District)await dataRepository.GetByIdAsync(key).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                PEMSEventSource.Log.DistrictServiceException(ex.Message, key.ToString());
                throw;
            }
        }

        /// <summary>
        /// Retrieve an item from the database with access check
        /// </summary>
        /// <param name="username">User name</param>
        /// <param name="key">Unique database item identifier, i.e. value of primary key</param>
        /// <returns>A database model object</returns>
        public async Task<District> GetByIdAsync(String username, Guid key)
        {
            AdminDto admin = null;
            try
            {
                // Access check
                admin = await UnitOfWork.AdminRepository.GetByUsernameAsync(username).ConfigureAwait(false);
                // Non-global admin can only get own district
                if (!IsAuthorized(admin, key))
                    throw new UnauthorizedAccessException("Unauthorized to retrieve the district.");

                return await GetByIdAsync(key).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var userId = (admin != null) ? admin.User.UserID.ToString() : null;
                PEMSEventSource.Log.DistrictServiceException(ex.Message, key.ToString(), userId, new LogRequest { Exception = ex });
                throw;
            }
        }

        /// <summary>
        /// Add new item to the database
        /// </summary>
        /// <param name="entity">New database model object</param>
        /// <returns>Updated database model object, e.g. with identity primary key populated</returns>
        public async Task<District> CreateAsync(District entity)
        {
            try
            {
                var dataRepository = UnitOfWork.GetDataRepository<DistrictDto, Guid>();
                return (District)await dataRepository.InsertAsync((DistrictDto)entity).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var districtId = (entity != null) ? entity.DistrictId.ToString() : null;
                PEMSEventSource.Log.DistrictServiceException(ex.Message, districtId, logRequest: new LogRequest { Exception = ex });
                throw;
            }
        }

        /// <summary>
        /// Add new item to the database with access check
        /// </summary>
        /// <param name="username">User name</param>
        /// <param name="entity">New database model object</param>
        /// <returns>Updated database model object, e.g. with identity primary key populated</returns>
        public async Task<District> CreateAsync(String username, District entity)
        {
            AdminDto admin = null;
            try
            {
                // Access check
                admin = await UnitOfWork.AdminRepository.GetByUsernameAsync(username).ConfigureAwait(false);
                // Non-global admin can only create own district
                if (!IsAuthorized(admin, entity.DistrictId))
                    throw new UnauthorizedAccessException("Unauthorized to add the district.");

                return await CreateAsync(entity).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var districtId = (entity != null) ? entity.DistrictId.ToString() : null;
                var userId = (admin != null) ? admin.User.UserID.ToString() : null;
                PEMSEventSource.Log.DistrictServiceException(ex.Message, districtId, userId, new LogRequest { Exception = ex });
                throw;
            }
        }

        /// <summary>
        /// Update existing item in the database
        /// </summary>
        /// <param name="entity">Database model object</param>
        /// <returns>Database model object</returns>
        public async Task<District> UpdateAsync(District entity)
        {
            District result;
            try
            {
                var dataRepository = UnitOfWork.GetDataRepository<DistrictDto, Guid>();
                result = (District)await dataRepository.UpdateAsync((DistrictDto)entity).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var districtId = (entity != null) ? entity.DistrictId.ToString() : null;
                PEMSEventSource.Log.DistrictServiceException(ex.Message, districtId, logRequest: new LogRequest { Exception = ex });
                throw;
            }
            return result;
        }

        /// <summary>
        /// Update existing item in the database with access check
        /// </summary>
        /// <param name="username">User name</param>
        /// <param name="entity">Database model object</param>
        /// <returns>Database model object</returns>
        public async Task<District> UpdateAsync(String username, District entity)
        {
            AdminDto admin = null;
            try
            {
                // Access check
                admin = await UnitOfWork.AdminRepository.GetByUsernameAsync(username).ConfigureAwait(false);
                // Non-global admin can only update own district
                if (!IsAuthorized(admin, entity.DistrictId))
                    throw new UnauthorizedAccessException("Unauthorized to update the district.");

                return await UpdateAsync(entity).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var districtId = (entity != null) ? entity.DistrictId.ToString() : null;
                var userId = (admin != null) ? admin.User.UserID.ToString() : null;
                PEMSEventSource.Log.DistrictServiceException(ex.Message, districtId, userId, new LogRequest { Exception = ex });
                throw;
            }
        }

        /// <summary>
        /// Delete existing item from the database
        /// </summary>
        /// <param name="key">Unique database item identifier, i.e. value of primary key</param>
        /// <returns>True if item was deleted successfully, false otherwise</returns>
        public async Task<Boolean> DeleteAsync(Guid key)
        {
            try
            {
                var dataRepository = UnitOfWork.GetDataRepository<DistrictDto, Guid>();
                return await dataRepository.DeleteAsync(key).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                PEMSEventSource.Log.DistrictServiceException(ex.Message, key.ToString(), logRequest: new LogRequest { Exception = ex });
                throw;
            }
        }

        /// <summary>
        /// Delete existing item from the database with access check
        /// </summary>
        /// <param name="username">User name</param>
        /// <param name="key">Unique database item identifier, i.e. value of primary key</param>
        /// <returns>True if item was deleted successfully, false otherwise</returns>
        public async Task<Boolean> DeleteAsync(String username, Guid key)
        {
            AdminDto admin = null;
            try
            {
                // Access check
                admin = await UnitOfWork.AdminRepository.GetByUsernameAsync(username).ConfigureAwait(false);
                // Non-global admin can only delete own district
                 if (!IsAuthorized(admin, key))
                    throw new UnauthorizedAccessException("Unauthorized to delete the district.");

                return await DeleteAsync(key).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var userId = (admin != null) ? admin.User.UserID.ToString() : null;
                PEMSEventSource.Log.DistrictServiceException(ex.Message, key.ToString(), userId, new LogRequest { Exception = ex });
                throw;
            }
        }

        /// <summary>
        /// Determine if user is authorized in this service
        /// </summary>
        /// <param name="admin">Admin DTO</param>
        /// <returns></returns>
        private Boolean IsAuthorized(AdminDto admin)
        {
            if (admin == null)
                return false;

            switch (admin.AdminType)
            {
                // Global admin and district admin
                case AdminType.GlobalAdmin:
                case AdminType.DistrictAdmin:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determine if user is authorized for the specified district
        /// </summary>
        /// <param name="admin">Admin DTO</param>
        /// <param name="districtId">District ID</param>
        /// <returns></returns>
        private Boolean IsAuthorized(AdminDto admin, Guid districtId)
        {
            if ((admin == null) || (districtId == default(Guid)))
                return false;

            switch (admin.AdminType)
            {
                // Global admin = all districts
                case AdminType.GlobalAdmin:
                    return true;
                // District admin = own district only
                case AdminType.DistrictAdmin:
                    return (admin.District.DistrictId == districtId);
                default:
                    return false;
            }
        }
    }
}
