using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Data.Repositories;

namespace PSoC.ManagementService.Data.IntegrationTests.Repositories
{
    [TestClass]
    public class ClassroomRepositoryTest
    {
        private ClassroomRepository _sut;

        [TestMethod]
        public async Task ClassroomRepository_DeleteAsync_ThrowsNotImplementedException()
        {
            Exception thrownException = null;
            try
            {
                var result = await _sut.DeleteAsync(Guid.NewGuid());
            }
            catch (Exception e)
            {
                thrownException = e;
            }

            // Assert
            Assert.IsNotNull(thrownException);
            Assert.IsInstanceOfType(thrownException, typeof(NotImplementedException));
        }

        [TestMethod]
        public async Task ClassroomRepository_DeleteManyAsync_ThrowsNotImplementedException()
        {
            Exception thrownException = null;
            try
            {
                var result = await _sut.DeleteAsync(new Guid[]{Guid.NewGuid()});
            }
            catch (Exception e)
            {
                thrownException = e;
            }

            // Assert
            Assert.IsNotNull(thrownException);
            Assert.IsInstanceOfType(thrownException, typeof(NotImplementedException));
        }

        [TestMethod]
        public async Task ClassroomRepository_GetAsync_ThrowsNotImplementedException()
        {
            Exception thrownException = null;
            try
            {
                var result = await _sut.GetAsync();
            }
            catch (Exception e)
            {
                thrownException = e;
            }

            // Assert
            Assert.IsNotNull(thrownException);
            Assert.IsInstanceOfType(thrownException, typeof(NotImplementedException));
        }

        [TestMethod]
        public async Task ClassroomRepository_GetByIdAsync_ThrowsNotImplementedException()
        {
            Exception thrownException = null;
            try
            {
                var result = await _sut.GetByIdAsync(Guid.NewGuid());
            }
            catch (Exception e)
            {
                thrownException = e;
            }

            // Assert
            Assert.IsNotNull(thrownException);
            Assert.IsInstanceOfType(thrownException, typeof(NotImplementedException));
        }

        [TestMethod]
        public async Task ClassroomRepository_InsertAsync_ThrowsNotImplementedException()
        {
            Exception thrownException = null;
            try
            {
                var result = await _sut.InsertAsync(new ClassroomDto());
            }
            catch (Exception e)
            {
                thrownException = e;
            }

            // Assert
            Assert.IsNotNull(thrownException);
            Assert.IsInstanceOfType(thrownException, typeof(NotImplementedException));
        }

        [TestMethod]
        public async Task ClassroomRepository_UpdateAsync_ThrowsNotImplementedException()
        {
            Exception thrownException = null;
            try
            {
                var result = await _sut.UpdateAsync(new ClassroomDto());
            }
            catch (Exception e)
            {
                thrownException = e;
            }

            // Assert
            Assert.IsNotNull(thrownException);
            Assert.IsInstanceOfType(thrownException, typeof(NotImplementedException));
        }

        [TestInitialize]
        public void Initialize()
        {
            _sut = new ClassroomRepository();
        }
    }
}