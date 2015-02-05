using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Services.Interfaces;
using PSoC.ManagementService.Services.Logging;
using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.Services
{
    /// <summary>
    /// User Service
    /// </summary>
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAdminRepository _adminRepository;

        public AdminService(IUnitOfWork unitOfWork, IAdminRepository adminRepository)
        {
            _unitOfWork = unitOfWork;
            _adminRepository = adminRepository;
        }

        public async Task<Admin> GetByUsernameAsync(string username)
        {
            Admin result = null;
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    var ex = new ArgumentException("Username is null,empty or white space", "username");
                    throw ex;
                }

                result = (Admin)await _adminRepository.GetByUsernameAsync(username).ConfigureAwait(false);
                return result;
            }
            catch (Exception ex)
            {
                var userId = (result != null) ? result.UserId.ToString() : null;
                PEMSEventSource.Log.AdminServiceException(ex.Message, new LogRequest { Exception = ex, UserId = userId });
                throw;
            }
        }

        public async Task UpdateLastLoginDateTime(Guid userId, DateTime loginDateTime)
        {
            try
            {
                if (userId == Guid.Empty)
                {
                    var ex = new ArgumentException("Value for User Id is invalid", "userId");
                    throw ex;
                }

                await _adminRepository.UpdateLastLoginDateTimeAsync(userId, loginDateTime).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                PEMSEventSource.Log.AdminServiceException(ex.Message, new LogRequest { Exception = ex, UserId = userId.ToString() });
                throw;
            }
        }

        public async Task<IEnumerable<Admin>> GetAsync()
        {
            List<Admin> admins;
            
            try
            {
                IList<AdminDto> adminDtos = await _adminRepository.GetAsync().ConfigureAwait(false);
                admins = adminDtos.Select(adminDto => (Admin)adminDto).ToList();
            }
            catch (Exception e)
            {
                PEMSEventSource.Log.AdminServiceException(e.Message, new LogRequest { Exception = e });
                admins = new List<Admin>();
            }
            
            return admins;
        }

        public async Task<Admin> GetByIdAsync(Guid userId)
        {
            Admin admin;

            try
            {
                AdminDto adminDto = await _adminRepository.GetByIdAsync(userId).ConfigureAwait(false);
                admin = (Admin) adminDto;
            }
            catch (Exception e)
            {
                PEMSEventSource.Log.AdminServiceException(e.Message, new LogRequest { Exception = e, UserId = userId.ToString() });
                admin = null;
            }

            return admin;
        }
        
        public async Task<bool> InsertAsync(Admin admin)
        {
            AdminDto adminDto;

            try
            {
                adminDto = (AdminDto) admin;
                adminDto = await _adminRepository.InsertAsync(adminDto).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                var userId = (admin != null) ? admin.UserId.ToString() : null;
                PEMSEventSource.Log.AdminServiceException(e.Message, new LogRequest { Exception = e, UserId = userId });
                adminDto = null;
            }

            return (adminDto != null);
        }

        public async Task<bool> UpdateAsync(Admin admin)
        {
            AdminDto adminDto;

            try
            {
                adminDto = (AdminDto)admin;
                adminDto = await _adminRepository.UpdateAsync(adminDto).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                var userId = (admin != null) ? admin.UserId.ToString() : null;
                PEMSEventSource.Log.AdminServiceException(e.Message, new LogRequest { Exception = e, UserId = userId });
                adminDto = null;
            }

            return (adminDto != null);
        }

        public async Task<bool> DeleteAsync(Guid userId)
        {
            bool deleted;

            try
            {
                deleted = await _adminRepository.DeleteAsync(userId).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                PEMSEventSource.Log.AdminServiceException(e.Message, new LogRequest { Exception = e, UserId = userId.ToString() });
                deleted = false;
            }

            return deleted;
        }
    }
}