using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PSoC.ManagementService.Services.Interfaces;
using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IAzureTableService _azureTableService;
        private readonly IConfigBase _configBase;
        private readonly string _customerTable;

        public CustomerService(IAzureTableService azureTableService, IConfigBase configBase)
        {
            _azureTableService = azureTableService;
            _configBase = configBase;
            _customerTable = configBase.GetApplicationSetting<string>("CustomerTableName");
        }

        public async Task<List<Customer>> GetAllCustomers()
        {
            try
            {
                return (await _azureTableService.GetAllEntities<Customer>(_customerTable)).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Customer> GetCustomerByPrimaryCustomerIdAsync(string primaryEnvironmentId, string customerName)
        {
            if (string.IsNullOrEmpty(primaryEnvironmentId))
            {
                throw new ArgumentNullException("primrayEnvironmentId");
            }

            if (string.IsNullOrEmpty(customerName))
            {
                throw new ArgumentNullException("Customer Name");
            }

            try
            {
                var customer = await _azureTableService.GetEntityByPartitionRowKeyAsync<Customer>(_customerTable, primaryEnvironmentId, customerName);
                return customer;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Customer> GetCustomerAsync(string environmentId)
        {
            if (string.IsNullOrEmpty(environmentId))
            {
                throw new ArgumentNullException("environmentId");
            }

            try
            {
                //Going to make the assumption here that there is only 1 customer per environment id
                var result = await _azureTableService.GetEntitiesByPartitionKey<Customer>(_customerTable, environmentId);
                var customer = result.SingleOrDefault();
                if (customer == null)
                {
                    //Ok lets check the associatedenvironmentIds... Azure does not support the contains expression
                    //so we will get all customers and check the list. There should not be many customers
                    var customers = (await _azureTableService.GetAllEntities<Customer>(_customerTable)).ToList();
                    customer = customers.SingleOrDefault(x => !String.IsNullOrEmpty(x.AssociatedEnvironmentIds) && 
                                    x.AssociatedEnvironmentIds.Contains(environmentId));
                }
                return customer;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> UpdateCustomerAsync(Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException("Customer");
            }

            try
            {
                var res = await _azureTableService.InsertOrReplaceEntityAsync<Customer>(_customerTable, customer);
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> DeleteCustomerAsync(Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException("Null Customer");
            }

            try
            {
                var res = await _azureTableService.DeleteEntityAsync<Customer>(_customerTable, customer);
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
