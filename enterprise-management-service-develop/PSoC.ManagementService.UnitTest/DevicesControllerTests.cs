using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PSoC.ManagementService.Controllers;
using PSoC.ManagementService.Services.Interfaces;

namespace PSoC.ManagementService.UnitTest
{
    //Note --- this tests all became invalided when the devices work was moved to the device service
    //Needs to be revisited

    //[TestClass]
    //public class DevicesControllerTests
    //{
    //    private DevicesController _controller;
    //    private Mock<IDeviceService> _mockDeviceService;
    //    private Mock<IConfigBase> _mockConfigBase;
    //    private Mock<ILicenseService> _mockLicenseService;

    //    [TestInitialize]
    //    public void TestInitialize()
    //    {
    //        _mockDeviceService = new Mock<IDeviceService>();
    //        _mockConfigBase = new Mock<IConfigBase>();
    //        _mockLicenseService = new Mock<ILicenseService>();
    //        _mockConfigBase.Setup(x => x.GetApplicationSetting<String>(It.IsAny<string>())).Returns("TableName");

    //        _controller = new DevicesController(_mockConfigBase.Object, _mockDeviceService.Object, _mockLicenseService.Object);
    //    }

    //    #region GET

    //    [TestMethod]
    //    public void Get_MissingEnvironmentClaims()
    //    {
    //        var claims = new List<Claim> {  };
    //        var identity = new ClaimsIdentity(claims);
    //        var claimsPrincipal = new ClaimsPrincipal(identity);
    //        _controller.User = claimsPrincipal;

    //        var result = _controller.Get();

    //        Assert.IsInstanceOfType(result, typeof(BadRequestResult));
    //    }

    //    [TestMethod]
    //    public void Get_EmptyResult()
    //    {
    //        _mockAzureTableService.Setup(x => x.GetEntitiesByPartitionKeys<Device>(It.IsAny<string>(), It.IsAny<List<string>>()))
    //            .Returns(Enumerable.Empty<Device>().AsQueryable());

    //        var claim = new Claim("EnvironmentIds", string.Empty);
    //        var claims = new List<Claim> { claim };
    //        var identity = new ClaimsIdentity(claims);
    //        var claimsPrincipal = new ClaimsPrincipal(identity);
    //        _controller.User = claimsPrincipal;

    //        var result = _controller.Get();
    //        var items = result as OkNegotiatedContentResult<IQueryable<Device>>;
    //        var itemsResult = items.Content;

    //        CollectionAssert.AreEqual(Enumerable.Empty<Device>().ToList(), itemsResult.ToList());
    //    }

    //    [TestMethod]
    //    public void Get_Success()
    //    {
    //        var expectedResult = new List<Device> 
    //        { 
    //            new Device
    //            {
    //                DeviceId = "1",
    //                DeviceName = "ABC"
    //            },
    //            new Device
    //            {
    //                DeviceId = "2",
    //                DeviceName = "Def"
    //            },
    //            new Device
    //            {
    //                DeviceId = "3",
    //                DeviceName = "XYZ"
    //            }
    //        };

    //        _mockAzureTableService.Setup(x => x.GetEntitiesByPartitionKeys<Device>(It.IsAny<string>(), It.IsAny<List<string>>()))
    //            .Returns(expectedResult.AsQueryable());

    //        var claim = new Claim("EnvironmentIds", string.Empty);
    //        var claims = new List<Claim> { claim };
    //        var identity = new ClaimsIdentity(claims);
    //        var claimsPrincipal = new ClaimsPrincipal(identity);
    //        _controller.User = claimsPrincipal;

    //        var result = _controller.Get();
    //        var items = result as OkNegotiatedContentResult<IQueryable<Device>>;
    //        var itemsResult = items.Content;

    //        CollectionAssert.AreEqual(expectedResult, itemsResult.ToList());
    //    }

    //    #endregion

    //    #region GETID

    //    [TestMethod]
    //    public void GetID_InvalidDeviceId()
    //    {
    //        var result = _controller.Get(null);

    //        Assert.IsInstanceOfType(result, typeof(BadRequestResult));
    //    }

    //    [TestMethod]
    //    public void GetID_NotFound()
    //    {
    //        _mockAzureTableService.Setup(x => x.GetEntitiesByRowKey<Device>(It.IsAny<string>(), It.IsAny<string>()))
    //                              .Returns<IEnumerable<Device>>(null);

    //        var result = _controller.Get("ABC");

    //        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    //    }

    //    #endregion

    //    #region PUT

    //    [TestMethod]
    //    public async Task Put_InvalidDeviceId()
    //    {
    //        var result = await _controller.Put(null, new Device());

    //        Assert.IsInstanceOfType(result, typeof(BadRequestResult));
    //    }

    //    [TestMethod]
    //    public async Task Put_InvalidEnvironmentId()
    //    {
    //        var result = await _controller.Put("ABC", new Device { EnvironmentId = null });

    //        Assert.IsInstanceOfType(result, typeof(BadRequestResult));
    //    }


    //    [TestMethod]
    //    public async Task Put_Success()
    //    {
    //        _mockAzureTableService.Setup(x => x.GetEntityByPartitionRowKeyAsync<Device>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
    //                              .Returns(Task.FromResult(new Device()));

    //        _mockAzureTableService.Setup(x => x.InsertOrReplaceEntityAsync<Device>(It.IsAny<string>(), It.IsAny<Device>()))
    //                              .Returns(Task.FromResult(true));

    //        var result = await _controller.Put("ABC", new Device { EnvironmentId = "DEF", DeviceId = "123" });

    //        Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<Device>));
    //    }



    //    #endregion

    //    #region DELETE

    //    [TestMethod]
    //    public async Task Delete_InvalidId()
    //    {
    //        var result = await _controller.Delete(null);

    //        Assert.IsInstanceOfType(result, typeof(BadRequestResult));
    //    }

    //    [TestMethod]
    //    public async Task Delete_Success()
    //    {
    //        var list = new List<Device> 
    //        { 
    //            new Device
    //            {
    //                DeviceId = "1",
    //                DeviceName = "ABC"
    //            },
    //            new Device
    //            {
    //                DeviceId = "2",
    //                DeviceName = "Def"
    //            }
    //        };

    //        _mockAzureTableService.Setup(x => x.GetEntitiesByRowKey<Device>(It.IsAny<string>(), It.IsAny<string>())).Returns(list.AsQueryable());
    //        _mockAzureTableService.Setup(x => x.DeleteBatchEntitiesAsync<Device>(It.IsAny<string>(), It.IsAny<List<Device>>())).Returns(Task.FromResult(true));

    //        var result = await _controller.Delete("ABC");

    //        Assert.IsInstanceOfType(result, typeof(OkResult));
    //    }

    //    #endregion
    //}
}
