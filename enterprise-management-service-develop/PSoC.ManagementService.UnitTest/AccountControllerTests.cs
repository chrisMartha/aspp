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

namespace PSoC.ManagementService.UnitTest
{
    [TestClass]
    public class AccountControllerTests
    {
        private Mock<IAdminService> _adminServiceMock;
        private Mock<IDistrictService> _districtServiceMock;
        private Mock<ISchoolnetService> _schoolnetServiceMock;
        private AccountController _sut;
        private const string ExpectedViewName = "Login";

        [TestInitialize]
        public void TestInitialize()
        {
            _adminServiceMock = new Mock<IAdminService>();
            _districtServiceMock = new Mock<IDistrictService>();
            _schoolnetServiceMock = new Mock<ISchoolnetService>();
            _sut = new AccountController(_adminServiceMock.Object, _schoolnetServiceMock.Object,
                _districtServiceMock.Object);
        }

        [TestMethod]
        public void AccountController_Login_Get_ShouldReturn_LoginPage()
        {
            //Arrange
            _sut.ViewData.ModelState.Clear();

            //Act
            var result = (ViewResult)_sut.Login();

            //Assert
            Assert.IsNotNull(result);
        }
         
        [TestMethod]
        public async Task AccountController_Login_Post_ShouldNotAccept_EmptyUsernameOrPassword()
        {
            //Arrange
            var loginModel = new LoginModel() { Username = string.Empty, Password = string.Empty };
            _sut.ViewData.ModelState.Clear();
            _sut.ViewData.ModelState.AddModelError("Error", "An Error Occured");            

            //Act
            var result = (ViewResult)await _sut.Login(loginModel);
            var modelState = _sut.ModelState;
           
            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(modelState.IsValid);
            Assert.AreEqual("The user name or password provided is incorrect.", modelState["LogOnError"].Errors[0].ErrorMessage);
            Assert.AreEqual(ExpectedViewName, result.ViewName);
            _sut.ViewData.ModelState.Clear();
        }

        [TestMethod]
        public async Task AccountController_Login_Post_ShouldReportAnError_ForANonWhiteListedUser()
        {
            //Arrange
            var loginModel = new LoginModel() { Username = "test123", Password = "test" };
            _adminServiceMock.Setup(x => x.GetByUsernameAsync(loginModel.Username)).ReturnsAsync(null);

            //Act
            var result = (ViewResult)await _sut.Login(loginModel);
            var modelState = _sut.ModelState;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(modelState.IsValid);
            Assert.AreEqual("Unauthorized to access this site.", modelState["LogOnError"].Errors[0].ErrorMessage);
            Assert.AreEqual(ExpectedViewName, result.ViewName);
        }

        [TestMethod]
        public async Task AccountController_Login_Post_ShouldReportAnError_ForAnInactiveUser()
        {
            //Arrange
            var loginModel = new LoginModel() { Username = "test123", Password = "test" };
            var admin = new Admin()
            {
                UserId = new Guid("7C25630E-FFE3-4D84-B42B-17C06A7109CB"),
                Username = "test123",
                UserType = "District Admin",
                Active = false,
                DistrictId = new Guid("00FCC2A7-A3D5-469C-A790-F2E8BCD44BCA")
            };

            _adminServiceMock.Setup(x => x.GetByUsernameAsync(loginModel.Username)).ReturnsAsync(admin);
        
            //Act
            var result = (ViewResult)await _sut.Login(loginModel);
            var modelState = _sut.ModelState;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(modelState.IsValid);
            Assert.AreEqual("Unauthorized to access this site.", modelState["LogOnError"].Errors[0].ErrorMessage);
            Assert.AreEqual(ExpectedViewName, result.ViewName);
        }

        [TestMethod]
        public async Task AccountController_Login_Post_ShouldReportAnError_DistrictNotFoundForANonGlobalAdmin()
        {
            //Arrange
            var loginModel = new LoginModel() { Username = "test123", Password = "test" };
            var admin = new Admin()
            {
                UserId = new Guid("7C25630E-FFE3-4D84-B42B-17C06A7109CB"),
                Username = "test123",
                UserType =  "District Admin",
                Active = true,
                DistrictId = new Guid("00FCC2A7-A3D5-469C-A790-F2E8BCD44BCA")
            };
            _sut.ViewData.ModelState.Clear();
            _adminServiceMock.Setup(x => x.GetByUsernameAsync(loginModel.Username)).ReturnsAsync(admin);
            _districtServiceMock.Setup(x => x.GetByIdAsync(loginModel.Username, admin.DistrictId.Value)).ReturnsAsync(null);

            //Act
            var result = (ViewResult)await _sut.Login(loginModel);
            var modelState = _sut.ModelState;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(modelState.IsValid);
            Assert.AreEqual("An unexpected critical error has occured. Please contact the site administrator.", modelState["ApplicationError"].Errors[0].ErrorMessage);
            Assert.AreEqual(ExpectedViewName, result.ViewName);
        }

        [TestMethod]
        public async Task AccountController_Login_Post_ShouldReportAnError_SNAuthFailure()
        {
            //Arrange
            var loginModel = new LoginModel() { Username = "test", Password = "test" };
            var admin = new Admin()
            {
                UserId = new Guid("7C25630E-FFE3-4D84-B42B-17C06A7109CB"),
                Username = "test",
                UserType = "Global Admin",
                Active = true
            };

            _sut.ViewData.ModelState.Clear();
            _adminServiceMock.Setup(x => x.GetByUsernameAsync(loginModel.Username)).ReturnsAsync(admin);
            _schoolnetServiceMock.Setup(x => x.IsAuthorizedAsync(
                    GlobalAppSettings.GetValue("OAuthUrl"),
                    GlobalAppSettings.GetValue("OAuthClientId"),
                    GlobalAppSettings.GetValue("OAuthApplicationId"),
                    loginModel.Username, loginModel.Password)).ReturnsAsync(false);

            //Act
            var result = (ViewResult)await _sut.Login(loginModel);
            var modelState = _sut.ModelState;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(modelState.IsValid);
            Assert.AreEqual("Failed to authorize with Schoolnet.", modelState["LogOnError"].Errors[0].ErrorMessage);
            Assert.AreEqual(ExpectedViewName, result.ViewName);
        }
       
    }
}
