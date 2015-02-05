using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Data.Repositories;

namespace PSoC.ManagementService.Data.IntegrationTests.Repositories
{
    [TestClass]
    public class CourseRepositoryTest
    {
        private CourseRepository _sut;

        [TestMethod]
        public async Task CourseRepository_DeleteAsync_ThrowsNotImplementedException()
        {
            Exception thrownException = null;
            try
            {
                var result = await _sut.DeleteAsync(Guid.NewGuid()).ConfigureAwait(false);
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
        public async Task CourseRepository_DeleteManyAsync_ThrowsNotImplementedException()
        {
            Exception thrownException = null;
            try
            {
                var result = await _sut.DeleteAsync(new Guid[] { Guid.NewGuid() }).ConfigureAwait(false);
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
        public async Task CourseRepository_GetAsync()
        {
            var result = await _sut.GetAsync().ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public async Task CourseRepository_GetByIdAsync()
        {
            var result = await _sut.GetByIdAsync(InitIntegrationTest.TestCourseLearningResourceId).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.CourseLearningResourceID == InitIntegrationTest.TestCourseLearningResourceId);
        }

        [TestMethod]
        public async Task CourseRepository_InsertAsync()
        {
            var dto = new CourseDto
            {
                CourseLearningResourceID = Guid.NewGuid(),
                Subject = "ELA",
                Grade = "01",

            };

            InitIntegrationTest.TestCourseLearningResourceIds.Add(dto.CourseLearningResourceID);

            var result = await _sut.InsertAsync(dto).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.CourseLearningResourceID == dto.CourseLearningResourceID);
        }

        [TestMethod]
        public async Task CourseRepository_UpdateAsync()
        {
            var dto = new CourseDto
            {
                CourseLearningResourceID = InitIntegrationTest.TestCourseLearningResourceId,
                Subject = "ELA",
                Grade = "09",
                CourseName = "9.1.1 Lesson 9",
            };

            var result = await _sut.UpdateAsync(dto).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.CourseLearningResourceID == dto.CourseLearningResourceID);
            Assert.IsTrue(result.CourseName == dto.CourseName);
        }

        [TestInitialize]
        public void Initialize()
        {
            _sut = new CourseRepository();
        }
    }
}