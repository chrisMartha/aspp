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
    /// Access point service
    /// </summary>
    public class AccessPointService : IAccessPointService
    {
        protected IUnitOfWork UnitOfWork { get; private set; }

        public AccessPointService(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        /// <summary>
        /// Retrieve all items from the database
        /// </summary>
        /// <returns>List of database model objects</returns>
        public async Task<IList<AccessPoint>> GetAsync()
        {
            try
            {
                var resultList = new List<AccessPoint>();

                var dataRepository = UnitOfWork.GetDataRepository<AccessPointDto, String>();
                foreach (var item in await dataRepository.GetAsync().ConfigureAwait(false))
                {
                    resultList.Add((AccessPoint)item);
                }

                return resultList;
            }
            catch (Exception ex)
            {
                PEMSEventSource.Log.AccessPointServiceException(ex.Message, logRequest: new LogRequest { Exception = ex });
                throw;
            }
        }

        /// <summary>
        /// Retrieve all items from the database with access check
        /// </summary>
        /// <param name="username">User name</param>
        /// <returns>List of database model objects</returns>
        public async Task<IList<AccessPoint>> GetAsync(String username)
        {
            AdminDto admin = null;

            try
            {
                var resultList = new List<AccessPoint>();

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
                        return await GetByDistrictAsync(admin.District.DistrictId).ConfigureAwait(false);
                    case AdminType.SchoolAdmin:
                        return await GetBySchoolAsync(admin.School.SchoolID).ConfigureAwait(false);
                }

                return resultList;
            }
            catch (Exception ex)
            {
                var userId = (admin != null) ? admin.User.UserID.ToString() : null;
                PEMSEventSource.Log.AccessPointServiceException(ex.Message, userId: userId, logRequest: new LogRequest { Exception = ex });
                throw;
            }
        }

        /// <summary>
        /// Retrieve all items from the database by district
        /// </summary>
        /// <param name="districtId">District ID</param>
        /// <returns>List of database model objects</returns>
        public async Task<IList<AccessPoint>> GetByDistrictAsync(Guid districtId)
        {
            try
            {
                var resultList = new List<AccessPoint>();

                foreach (var item in await UnitOfWork.AccessPointRepository.GetByDistrictAsync(districtId).ConfigureAwait(false))
                {
                    resultList.Add((AccessPoint)item);
                }

                return resultList;
            }
            catch (Exception ex)
            {
                PEMSEventSource.Log.AccessPointServiceException(ex.Message, logRequest: new LogRequest { Exception = ex, DistrictId = districtId.ToString() });
                throw;
            }
        }

        /// <summary>
        /// Retrieve all items from the database by school
        /// </summary>
        /// <param name="schoolId">School ID</param>
        /// <returns>List of database model objects</returns>
        public async Task<IList<AccessPoint>> GetBySchoolAsync(Guid schoolId)
        {
            try
            {
                var resultList = new List<AccessPoint>();

                foreach (var item in await UnitOfWork.AccessPointRepository.GetBySchoolAsync(schoolId).ConfigureAwait(false))
                {
                    resultList.Add((AccessPoint)item);
                }

                return resultList;
            }
            catch (Exception ex)
            {
                PEMSEventSource.Log.AccessPointServiceException(ex.Message, logRequest: new LogRequest { Exception = ex, SchoolId = schoolId.ToString() });
                throw;
            }
        }

        /// <summary>
        /// Retrieve an item from the database
        /// </summary>
        /// <param name="key">Unique database item identifier, i.e. value of primary key</param>
        /// <returns>A database model object</returns>
        public async Task<AccessPoint> GetByIdAsync(String key)
        {
            try
            {
                var dataRepository = UnitOfWork.GetDataRepository<AccessPointDto, String>();
                return (AccessPoint)await dataRepository.GetByIdAsync(key).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                PEMSEventSource.Log.AccessPointServiceException(ex.Message, key, logRequest: new LogRequest { Exception = ex });
                throw;
            }
        }

        /// <summary>
        /// Retrieve an item from the database with access check
        /// </summary>
        /// <param name="username">User name</param>
        /// <param name="key">Unique database item identifier, i.e. value of primary key</param>
        /// <returns>A database model object</returns>
        public async Task<AccessPoint> GetByIdAsync(String username, String key)
        {
            AdminDto admin = null;
            try
            {
                // Access check
                admin = await UnitOfWork.AdminRepository.GetByUsernameAsync(username).ConfigureAwait(false);
                if (!IsAuthorized(admin))
                    throw new UnauthorizedAccessException("Unauthorized to retrieve the access point.");

                var result = await GetByIdAsync(key).ConfigureAwait(false);

                // Non-global admin can only get authorized access point
                if (!IsAuthorized(admin, result))
                    throw new UnauthorizedAccessException("Unauthorized to retrieve the access point.");
                
                return result;
            }
            catch (Exception ex)
            {
                var userId = (admin != null) ? admin.User.UserID.ToString() : null;
                PEMSEventSource.Log.AccessPointServiceException(ex.Message, key, userId, new LogRequest { Exception = ex });
                throw;
            }
        }

        /// <summary>
        /// Add new item to the database
        /// </summary>
        /// <param name="entity">New database model object</param>
        /// <returns>Updated database model object, e.g. with identity primary key populated</returns>
        public async Task<AccessPoint> CreateAsync(AccessPoint entity)
        {
            try
            {
                var dataRepository = UnitOfWork.GetDataRepository<AccessPointDto, String>();
                return (AccessPoint)await dataRepository.InsertAsync((AccessPointDto)entity).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var accessPointId = (entity != null) ? entity.WifiBSSId : null;
                PEMSEventSource.Log.AccessPointServiceException(ex.Message, accessPointId, logRequest: new LogRequest { Exception = ex });
                throw;
            }
        }

        /// <summary>
        /// Add new item to the database with access check
        /// </summary>
        /// <param name="username">User name</param>
        /// <param name="entity">New database model object</param>
        /// <returns>Updated database model object, e.g. with identity primary key populated</returns>
        public async Task<AccessPoint> CreateAsync(String username, AccessPoint entity)
        {
            AdminDto admin = null;
            try
            {
                // Access check
                admin = await UnitOfWork.AdminRepository.GetByUsernameAsync(username).ConfigureAwait(false);

                // Non-global admin can only create authorized access point
                if (!IsAuthorized(admin, entity))
                {
                    throw new UnauthorizedAccessException("Unauthorized to add the access point.");
                }

                return await CreateAsync(entity).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var accessPointId = (entity != null) ? entity.WifiBSSId : null;
                var userId = (admin != null) ? admin.User.UserID.ToString() : null;
                PEMSEventSource.Log.AccessPointServiceException(ex.Message, accessPointId, userId, new LogRequest { Exception = ex });
                throw;
            }
        }

        /// <summary>
        /// Update existing item in the database
        /// </summary>
        /// <param name="entity">Database model object</param>
        /// <returns>Database model object</returns>
        public async Task<AccessPoint> UpdateAsync(AccessPoint entity)
        {
            try
            {
                var dataRepository = UnitOfWork.GetDataRepository<AccessPointDto, String>();
                return (AccessPoint)await dataRepository.UpdateAsync((AccessPointDto)entity).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var accessPointId = (entity != null) ? entity.WifiBSSId : null;
                PEMSEventSource.Log.AccessPointServiceException(ex.Message, accessPointId, logRequest: new LogRequest { Exception = ex });
                throw;
            }
        }

        /// <summary>
        /// Update existing item in the database with access check
        /// </summary>
        /// <param name="username">User name</param>
        /// <param name="entity">Database model object</param>
        /// <returns>Database model object</returns>
        public async Task<AccessPoint> UpdateAsync(String username, AccessPoint entity)
        {
            AdminDto admin = null;
            try
            {
                // Access check
                admin = await UnitOfWork.AdminRepository.GetByUsernameAsync(username).ConfigureAwait(false);
 
                // Non-global admin can only update authorized access point
                if (!IsAuthorized(admin, entity))
                    throw new UnauthorizedAccessException("Unauthorized to update the access point.");

                return await UpdateAsync(entity).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var accessPointId = (entity != null) ? entity.WifiBSSId : null;
                var userId = (admin != null) ? admin.User.UserID.ToString() : null;
                PEMSEventSource.Log.AccessPointServiceException(ex.Message, accessPointId, userId, new LogRequest { Exception = ex });
                throw;
            }
        }

        /// <summary>
        /// Delete existing item from the database
        /// </summary>
        /// <param name="key">Unique database item identifier, i.e. value of primary key</param>
        /// <returns>True if item was deleted successfully, false otherwise</returns>
        public async Task<Boolean> DeleteAsync(String key)
        {
            try
            {
                var dataRepository = UnitOfWork.GetDataRepository<AccessPointDto, String>();
                return await dataRepository.DeleteAsync(key).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                PEMSEventSource.Log.AccessPointServiceException(ex.Message, key, logRequest: new LogRequest { Exception = ex });
                throw;
            }
        }

        /// <summary>
        /// Delete existing item from the database with access check
        /// </summary>
        /// <param name="username">User name</param>
        /// <param name="key">Unique database item identifier, i.e. value of primary key</param>
        /// <returns>True if item was deleted successfully, false otherwise</returns>
        public async Task<Boolean> DeleteAsync(String username, String key)
        {
            AdminDto admin = null;
            try
            {
                // Access check
                admin = await UnitOfWork.AdminRepository.GetByUsernameAsync(username).ConfigureAwait(false);
                if (!IsAuthorized(admin))
                    throw new UnauthorizedAccessException("Unauthorized to delete the access point.");

                // Get access point info
                var entity = await GetByIdAsync(username, key).ConfigureAwait(false);

                // Non-global admin can only delete authorized access point
                if (!IsAuthorized(admin, entity))
                    throw new UnauthorizedAccessException("Unauthorized to delete the access point.");

                return await DeleteAsync(key).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var userId = (admin != null) ? admin.User.UserID.ToString() : null;
                PEMSEventSource.Log.AccessPointServiceException(ex.Message, key, userId, new LogRequest { Exception = ex });
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
                // Global admin, district admin, school admin
                case AdminType.GlobalAdmin:
                case AdminType.DistrictAdmin:
                case AdminType.SchoolAdmin:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determine if user is authorized for the specified access point
        /// </summary>
        /// <param name="admin">Admin DTO</param>
        /// <param name="accessPoint">Access point</param>
        /// <returns></returns>
        private Boolean IsAuthorized(AdminDto admin, AccessPoint accessPoint)
        {
            if ((admin == null) || (accessPoint == null))
                return false;

            switch (admin.AdminType)
            {
                // Global admin = all access points
                case AdminType.GlobalAdmin:
                    return true;
                // District admin = access points of own district only
                case AdminType.DistrictAdmin:
                    return (admin.District.DistrictId == accessPoint.DistrictId);
                // School admin = access points of own school only
                case AdminType.SchoolAdmin:
                    return (admin.School.SchoolID == accessPoint.SchoolId);
                default:
                    return false;
            }
        }
    }
}
