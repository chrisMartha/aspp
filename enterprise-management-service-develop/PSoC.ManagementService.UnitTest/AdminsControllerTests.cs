using System;
using System.Threading.Tasks;
using System.Web.Mvc;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using PSoC.ManagementService.Controllers;
using PSoC.ManagementService.Core;
using PSoC.ManagementService.Models;
using PSoC.ManagementService.Services.Interfaces;
using PSoC.ManagementService.Services.Models;


namespace PSoC.ManagementService.Tests
{
    [TestClass]
    public class AdminsControllerTests
    {
        private Mock<IAdminService> _adminServiceMock;
        private AdminsController _sut;

        [TestInitialize]
        public void TestInitialize()
        {
            _adminServiceMock = new Mock<IAdminService>();;
            _sut = new AdminsController(_adminServiceMock.Object);
        }

        [TestMethod]
        public async Task AccountController_Index_Get_ShouldReturn_IndexPage()
        {
            //Arrange
            _sut.ViewData.ModelState.Clear();

            //Act
            var result = await _sut.Index().ConfigureAwait(false) as ViewResult;

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task AccountController_Edit_ShouldReturn_HttpNotFoundResult()
        {
            //Arrange
            var userId = Guid.Parse("c89ccc4e-edb6-43c9-9dbe-e46cdb845fae");

            //Act
            var result = await _sut.Edit(userId).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(System.Web.Mvc.HttpNotFoundResult));
        }

        [TestMethod]
        public void AccountController_Create_ShouldReturn_CreatePage()
        {
            //Act
            var result =  _sut.Create() as ViewResult;

            //Assert
            Assert.IsNotNull(result);
        }  
    }
}
