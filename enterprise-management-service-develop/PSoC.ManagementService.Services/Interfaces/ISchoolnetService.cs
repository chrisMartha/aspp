using System.Threading.Tasks;

namespace PSoC.ManagementService.Services.Interfaces
{
    /// <summary>
    /// Interface for Schoolnet
    /// </summary>
    public interface ISchoolnetService
    {
        /// <summary>
        /// Authorize with Schoolnet
        /// </summary>
        /// <param name="restUrl">Represents OAuthUrl</param>
        /// <param name="clientId">Represents OAuthClientId</param>
        /// <param name="clientSecret">Represents OAuthApplicationId</param>
        /// <param name="username">Represents Pems/Schoolnet Username</param>
        /// <param name="password">Represents Schoolnet Password</param>
        /// <returns></returns>
        Task<bool> IsAuthorizedAsync(string restUrl, string clientId, string clientSecret, string username,
            string password);
    }
}
