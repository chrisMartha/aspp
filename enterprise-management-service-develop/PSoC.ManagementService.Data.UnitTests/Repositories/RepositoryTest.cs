using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSoC.ManagementService.Data.DataMapper;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Data.QueryFactory;
using PSoC.ManagementService.Data.Repositories;

namespace PSoC.ManagementService.Data.UnitTests.Repositories
{
    [TestClass]
    public class RepositoryTest
    {
        [TestMethod]
        public void Repository_GetKeyProperty_TableWithIndependentPrimaryKey()
        {
            // Arrange
            Repository<LicenseRequestDto, LicenseRequestQuery, LicenseRequestDataMapper, Guid> sut = new LicenseRequestRepository();

            // Act
            PropertyInfo licenseRequestPrimaryKeyPropertyInfo = sut.GetKeyProperty();

            // Assert
            Assert.AreEqual(licenseRequestPrimaryKeyPropertyInfo.Name, "LicenseRequestID");
            Assert.AreEqual(licenseRequestPrimaryKeyPropertyInfo.PropertyType, typeof(Guid));
        }

        [TestMethod]
        public void Repository_GetKeyProperty_TableWithDependentPrimaryKey()
        {
            // Arrange
            Repository<LicenseDto, LicenseQuery, LicenseDataMapper, Guid> sut = new LicenseRepository();

            // Act
            PropertyInfo licensePrimaryKeyPropertyInfo = sut.GetKeyProperty();

            // Assert
            Assert.AreEqual(licensePrimaryKeyPropertyInfo.Name, "LicenseRequestID");
            Assert.AreEqual(licensePrimaryKeyPropertyInfo.PropertyType, typeof(Guid));
        }
    }
}
