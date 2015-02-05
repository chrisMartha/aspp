using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PSoC.ManagementService.Services.Interfaces;
using System.Threading.Tasks;
using PSoC.ManagementService.Services.Models;
using System.Linq;
using System.Collections.Generic;

namespace PSoC.ManagementService.Services.Test
{
    [TestClass]
    public class CustomerServiceTests
    {
        private CustomerService _customerService;
        private Mock<IConfigBase> _mockConfigBase;
        private Mock<IAzureTableService> _mockAzureService;


        [TestInitialize]
        public void TestInitialize()
        {
            _mockConfigBase = new Mock<IConfigBase>();
            _mockAzureService = new Mock<IAzureTableService>();

            _mockConfigBase.Setup(x => x.GetApplicationSetting<String>(It.IsAny<string>())).Returns("TableName");

            _customerService = new CustomerService(_mockAzureService.Object, _mockConfigBase.Object);
        }

        #region GetAllCustomers

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task GetAllCustomers_Exception()
        {
            _mockAzureService.Setup(x => x.GetAllEntities<Customer>(It.IsAny<string>())).Throws(new Exception());

            await _customerService.GetAllCustomers();
        }

        [TestMethod]
        public async Task GetAllCustomers_EmptyList()
        {
            var expectedRes = Enumerable.Empty<Customer>().ToList();

            _mockAzureService.Setup(x => x.GetAllEntities<Customer>(It.IsAny<string>())).Returns(Task.FromResult(expectedRes.AsQueryable()));

            var res = await _customerService.GetAllCustomers();

            Assert.IsNotNull(res);
            Assert.IsTrue(res.Count == 0);
        }

        [TestMethod]
        public async Task GetAllCustomers_Success()
        {
            var expectedRes = new List<Customer> 
            { 
                new Customer{ EnvironmentId = "1"},
                new Customer{ EnvironmentId = "2"}
            };

            _mockAzureService.Setup(x => x.GetAllEntities<Customer>(It.IsAny<string>())).Returns(Task.FromResult(expectedRes.AsQueryable()));

            var res = await _customerService.GetAllCustomers();

            Assert.IsNotNull(res);
            Assert.IsTrue(res.Count == expectedRes.Count);
            CollectionAssert.AllItemsAreNotNull(res);
        }

        #endregion

        #region GetCustomerByPrimaryCustomerIdAsync

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task GetCustomerByPrimaryCustomerIdAsync_InvalidPrimary()
        {
            await _customerService.GetCustomerByPrimaryCustomerIdAsync(null, "abc");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task GetCustomerByPrimaryCustomerIdAsync_InvalidCustomerName()
        {
            await _customerService.GetCustomerByPrimaryCustomerIdAsync("abc", null);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task GetCustomerByPrimaryCustomerIdAsync_Exception()
        {
            _mockAzureService.Setup(x => x.GetEntityByPartitionRowKeyAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());

            await _customerService.GetCustomerByPrimaryCustomerIdAsync("abc", "def");
        }

        [TestMethod]
        public async Task GetCustomerByPrimaryCustomerIdAsync_NotFound()
        {
            _mockAzureService.Setup(x => x.GetEntityByPartitionRowKeyAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult<Customer>(null));

            var res = await _customerService.GetCustomerByPrimaryCustomerIdAsync("abc", "def");

            Assert.IsNull(res);
        }

        [TestMethod]
        public async Task GetCustomerByPrimaryCustomerIdAsync_Success()
        {
            var customer = new Customer { EnvironmentId = "abc", CustomerName = "def" };

            _mockAzureService.Setup(x => x.GetEntityByPartitionRowKeyAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult<Customer>(customer));

            var res = await _customerService.GetCustomerByPrimaryCustomerIdAsync("abc", "def");

            Assert.AreEqual(customer, res);
        }

        #endregion

        #region GetCustomerAsync

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task GetCustomerAsync_NullEnvironmentId()
        {
            await _customerService.GetCustomerAsync(null);
        }


        #endregion

        #region UpdateCustomerAsync

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task UpdateCustomerAsync_NullCustomer()
        {
            await _customerService.UpdateCustomerAsync(null);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task UpdateCustomerAsync_Exception()
        {
            _mockAzureService.Setup(x => x.InsertOrReplaceEntityAsync<Customer>(It.IsAny<string>(), It.IsAny<Customer>()))
               .Throws(new Exception());

            await _customerService.UpdateCustomerAsync(new Customer());
        }

        [TestMethod]
        public async Task UpdateCustomerAsync_Failure()
        {
            _mockAzureService.Setup(x => x.InsertOrReplaceEntityAsync<Customer>(It.IsAny<string>(), It.IsAny<Customer>()))
               .Returns(Task.FromResult(false));

            var res = await _customerService.UpdateCustomerAsync(new Customer());

            Assert.IsFalse(res);
        }

        [TestMethod]
        public async Task UpdateCustomerAsync_Success()
        {
            _mockAzureService.Setup(x => x.InsertOrReplaceEntityAsync<Customer>(It.IsAny<string>(), It.IsAny<Customer>()))
               .Returns(Task.FromResult(true));

            var res = await _customerService.UpdateCustomerAsync(new Customer());

            Assert.IsTrue(res);
        }

        #endregion

        #region DeleteCustomerAsync

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task DeleteCustomerAsync_NullCustomer()
        {
            await _customerService.DeleteCustomerAsync(null);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task DeleteCustomerAsync_Exception()
        {
            _mockAzureService.Setup(x => x.DeleteEntityAsync(It.IsAny<string>(), It.IsAny<Customer>())).Throws(new Exception());

            await _customerService.DeleteCustomerAsync(new Customer());
        }

        [TestMethod]
        public async Task DeleteCustomerAsync_Success()
        {
            var customer = new Customer { EnvironmentId = "ABC" };

            _mockAzureService.Setup(x => x.DeleteEntityAsync(It.IsAny<string>(), customer)).Returns(Task.FromResult(true));

            var res = await _customerService.DeleteCustomerAsync(customer);

            Assert.IsTrue(res);
        }

        #endregion

    }
}
