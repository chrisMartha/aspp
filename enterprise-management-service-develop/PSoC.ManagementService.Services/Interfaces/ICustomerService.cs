using System.Collections.Generic;
using System.Threading.Tasks;
using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<List<Customer>> GetAllCustomers();
        Task<Customer> GetCustomerByPrimaryCustomerIdAsync(string primrayEnvironmentId, string customerName);
        Task<Customer> GetCustomerAsync(string environmentId);
        Task<bool> UpdateCustomerAsync(Customer customer);
        Task<bool> DeleteCustomerAsync(Customer customer);
    }
}
