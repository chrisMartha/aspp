using System;
using System.Threading.Tasks;
using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Services.Interfaces;
using PSoC.ManagementService.Services.Logging;
using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.Services
{
    /// <summary>
    /// User service
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> GetUserAsync(Guid userId, LogRequest logRequest)
        {
            try
            {
                if (userId == Guid.Empty)
                {
                    throw new ArgumentException("Value for user Id is invalid", "userId");
                }

                var result = (User)await _userRepository.GetByIdAsync(userId).ConfigureAwait(false);
                return result;
            }
            catch (Exception ex)
            {
                logRequest = logRequest ?? new LogRequest();
                logRequest.Exception = ex;
                PEMSEventSource.Log.UserServiceException(ex.Message, logRequest);
                throw;
            }
        }

        public async Task<User> InsertUserAsync(User user, LogRequest logRequest)
        {
            try
            {
                if (user == null || user.UserId == Guid.Empty)
                {
                    throw new ArgumentException("Value for user or user Id is invalid", "user");
                }

                return (User)await _userRepository.InsertAsync((UserDto)user).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logRequest = logRequest ?? new LogRequest();
                logRequest.Exception = ex;
                PEMSEventSource.Log.UserServiceException(ex.Message, logRequest);
                throw;
            }
        }
    }
}