using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using PSoC.ManagementService.Data.Helpers;
using PSoC.ManagementService.Data.Models;

namespace PSoC.ManagementService.Data.IntegrationTests
{
    [TestClass]
    public class InitIntegrationTest
    {
        public const string TestWifiBSSID = "19:99:18:A4:DF:47";

        public static readonly Guid TestCourseLearningResourceId = Guid.Parse("{2DE739B3-23C2-405F-84DF-959DFA332A6A}");
        public static readonly Guid TestDeleteDistrictId = Guid.Parse("{fb0f6dd6-c05c-4690-869e-3221de8a15c3}");
        public static readonly Guid TestDeviceId = Guid.Parse("{24247461-1CC5-4660-842D-FA8E72927AD8}");
        public static readonly Guid TestDistrictId = Guid.Parse("{C8AD1CC7-4B2B-4183-A624-A8C68C365E30}");
        public static readonly Guid TestLicenseRequestId = Guid.Parse("{61392399-c1c4-4583-9014-018cdca56718}");
        public static readonly Guid TestLicenseRequestIdForLicenseInsert = Guid.Parse("{5b5ce4aa-0ce5-4e9c-9572-1883d93fe152}");
        public static readonly Guid TestSchoolId = Guid.Parse("{3FC43BD8-7F81-428F-9F25-279737124610}");
        public static readonly Guid TestTeacherId = Guid.Parse("{4aa192a9-a3e3-4d69-9bf1-5a020451c1d9}");

        public static List<string> TestAccessPointIds = new List<string>();
        public static List<Guid> TestClassroomIds = new List<Guid>();
        public static List<Guid> TestCourseLearningResourceIds = new List<Guid>();
        public static List<Guid> TestDeviceIds = new List<Guid>();
        public static List<Guid> TestDistrictIds = new List<Guid>();
        public static List<Guid> TestLicenseRequestIds = new List<Guid>();
        public static List<Guid> TestUserIds = new List<Guid>();

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            Console.WriteLine("AssemblyCleanup");
            Console.WriteLine("Clear any Test Data ");
            ClearTestData().Wait();
        }

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            Console.WriteLine("AssemblyInit ");

            Console.WriteLine("Clear any Test Data ");
            ClearTestData().Wait();

            Console.WriteLine("Load Test Data ");
            LoadTestData().Wait();
        }

        private static async Task ClearTestData()
        {
            string query = File.ReadAllText("./SQL/dbo.Delete.Test.Data.sql");

            if (TestCourseLearningResourceIds.Count > 0)
            {
                string ids = String.Join(",", TestCourseLearningResourceIds
                    .Select(g => "'" + g.ToString("D") + "'")
                    .ToArray());

                query += Environment.NewLine
                    + string.Format(
                      "DELETE FROM [dbo].[DeviceInstalledCourse] WHERE [CourseLearningResourceID] IN ({0})" + Environment.NewLine
                    + "DELETE FROM [dbo].[Course] WHERE [CourseLearningResourceID] IN ({0})", ids);
            }

            if (TestLicenseRequestIds.Count > 0)
            {
                string ids = String.Join(",", TestLicenseRequestIds
                   .Select(g => "'" + g.ToString("D") + "'")
                   .ToArray());

                query += Environment.NewLine
                    + string.Format(
                      "DELETE FROM [dbo].[License] WHERE [LicenseRequestID] In ({0})" + Environment.NewLine
                    + "DELETE FROM [dbo].[LicenseRequest] WHERE [LicenseRequestID] In ({0})", ids);
            }

            if (TestDeviceIds.Count > 0)
            {
                string ids = String.Join(",", TestDeviceIds
                    .Select(g => "'" + g.ToString("D") + "'")
                    .ToArray());

                query += Environment.NewLine
                   + string.Format(
                       "DELETE FROM [dbo].[DeviceInstalledCourse] WHERE [DeviceId] In ({0})" + Environment.NewLine
                     + "DELETE FROM [dbo].[Device] WHERE [DeviceId] In ({0})", ids);
            }

            if (TestAccessPointIds.Count > 0)
            {
                string ids = String.Join(",", TestAccessPointIds.Select(a => "'" + a + "'").ToArray());

                query += Environment.NewLine
                   + string.Format(
                     "DELETE FROM [dbo].[AccessPoint] WHERE [WifiBSSID] In ({0})", ids);
            }

            if (TestClassroomIds.Count > 0)
            {
                string ids = String.Join(",", TestClassroomIds
                    .Select(g => "'" + g.ToString("D") + "'")
                    .ToArray());

                query += Environment.NewLine
                   + string.Format(
                     "DELETE FROM [dbo].[Classroom] WHERE [ClassroomID] In ({0})", ids);
            }

            if (TestDistrictIds.Count > 0)
            {
                string ids = String.Join(",", TestDistrictIds
                    .Select(g => "'" + g.ToString("D") + "'")
                    .ToArray());

                query += Environment.NewLine
                    + string.Format(
                      "DELETE FROM [dbo].[School] WHERE [DistrictID] In ({0})" + Environment.NewLine
                    + "DELETE FROM [dbo].[District] WHERE [DistrictID] In ({0})", ids);
            }

            if(TestUserIds.Count>0)
            {
                string ids = String.Join(",", TestUserIds
                    .Select(g => "'" + g.ToString("D") + "'")
                    .ToArray());

                query += Environment.NewLine
                   + string.Format(
                     "DELETE FROM [dbo].[User] WHERE [UserID] In ({0})", ids);
            }

            await DataAccessHelper.ExecuteAsync(query).ConfigureAwait(false);
        }

        private static async Task LoadTestData()
        {
            string query = File.ReadAllText("./SQL/dbo.Insert.Test.Data.sql");

            await DataAccessHelper.ExecuteAsync(query).ConfigureAwait(false);
        }
    }
}