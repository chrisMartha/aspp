using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using PSoC.ManagementService.Data.Helpers;
using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Data.Types;

namespace PSoC.ManagementService.Data.QueryFactory
{
    public class LicenseRequestQuery : IQueryFactory<LicenseRequestDto, Guid>
    {
        public QueryObject GetDeleteManyQuery(Guid[] keys)
        {
            var dt = new DataTable();
            dt.Columns.Add("Item", typeof(Guid));

            foreach (var key in keys)
                dt.Rows.Add(key);

            var paramList = new List<SqlParameter>
            {
                 new SqlParameter("@idList", SqlDbType.Structured) { TypeName = "dbo.GuidListTableType", Value = dt }
            };

            const string query = @"
                BEGIN TRANSACTION
                    DELETE FROM [dbo].[License] WHERE LicenseRequestID IN (SELECT il.Item FROM @idList il);
                    DELETE FROM [dbo].[LicenseRequest] WHERE LicenseRequestID IN (SELECT il.Item FROM @idList il);
                COMMIT TRANSACTION";

            return new QueryObject { QueryString = query, SqlParameters = paramList };
        }

        public QueryObject GetDeleteQuery(Guid key)
        {
            string query = @"BEGIN TRANSACTION
                                DELETE FROM [dbo].[License] WHERE LicenseRequestID = @LicenseRequestID
                                DELETE FROM [dbo].[LicenseRequest] WHERE LicenseRequestID = @LicenseRequestID
                            COMMIT TRANSACTION";

            var paramList = new List<SqlParameter>
            {
                new SqlParameter("@LicenseRequestID", SqlDbType.UniqueIdentifier) { Value =  key},
            };

            return new QueryObject { QueryString = query, SqlParameters = paramList };
        }

        public QueryObject GetInsertQuery(LicenseRequestDto entity)
        {
            return GetInsertQuery(entity, false);
        }

        public IList<SqlParameter> GetParameterList(LicenseRequestDto entity)
        {
            var school = entity.School ?? entity.Device.School;

            var paramList = new List<SqlParameter>
            {
                new SqlParameter("@LicenseRequestID", SqlDbType.UniqueIdentifier) { Value =  entity.LicenseRequestID},
                new SqlParameter("@DistrictID", SqlDbType.UniqueIdentifier) { Value = (school == null) ? DBNull.Value : (object) school.District.DistrictId, IsNullable = true },
                new SqlParameter("@SchoolID", SqlDbType.UniqueIdentifier) { Value = (school == null) ? DBNull.Value : (object) school.SchoolID, IsNullable = true },
                new SqlParameter("@ConfigCode", SqlDbType.NVarChar) { Value = entity.ConfigCode },
                new SqlParameter("@WifiBSSID", SqlDbType.Char) { Value = entity.AccessPoint.WifiBSSID },
                new SqlParameter("@LicenseRequestTypeID", SqlDbType.Int) { Value = (int)entity.LicenseRequestType},
                new SqlParameter("@DeviceID", SqlDbType.UniqueIdentifier) { Value = entity.Device.DeviceID },
                new SqlParameter("@UserID", SqlDbType.UniqueIdentifier) { Value = entity.User.UserID },
                new SqlParameter("@RequestDateTime", SqlDbType.DateTime) { Value = entity.RequestDateTime },
                new SqlParameter("@Response", SqlDbType.NVarChar) { Value = entity.Response.NullIfEmpty(), IsNullable =true},
                new SqlParameter("@ResponseDateTime", SqlDbType.DateTime) { Value = entity.ResponseDateTime.NullIfEmpty(), IsNullable =true },
                new SqlParameter("@LocationId", SqlDbType.NVarChar) { Value = entity.LocationId.NullIfEmpty(), IsNullable =true},
                new SqlParameter("@LocationName", SqlDbType.NVarChar) { Value = entity.LocationName.NullIfEmpty(), IsNullable =true },
                new SqlParameter("@LearningContentQueued", SqlDbType.Int) { Value = entity.LearningContentQueued.NullIfEmpty(), IsNullable =true },
             };

            return paramList;
        }

        public QueryObject GetSelectQuery(string whereClause = "", ICollection<SqlParameter> parameters = null, bool loadNestedTypes = false)
        {
            string query = @"
                    DECLARE @tmpLicense Table
                    (
                        [LicenseRequestID] [uniqueidentifier] NOT NULL,
                        [ConfigCode] [nvarchar](50) NOT NULL,
                        [RequestDateTime] [datetime] NOT NULL,
                        [LocationId] [nvarchar](50) NULL,
                        [LocationName] [nvarchar](50) NULL,
                        [LearningContentQueued] [int] NULL,
                        [LicenseRequestCreated] [datetime2](7) NULL,
                        [LicenseIssueDateTime] [datetime]  NULL,
                        [LicenseExpiryDateTime] [datetime] NULL,
                        [LicenseCreated] [datetime2](7) NULL,
                        [DeviceID] [uniqueidentifier] NULL,
                        [DeviceName] BINARY (256) NULL,
                        [DeviceType] [nvarchar](50) NULL,
                        [DeviceOS] [nvarchar](50) NULL,
                        [DeviceOSVersion] [nvarchar](50) NULL,
                        [PSoCAppID] [nvarchar](50) NULL,
                        [PSoCAppVersion] [nvarchar](50) NULL,
                        [DeviceFreeSpace] [bigint] NULL,
                        [ConfiguredGrades] [nvarchar](50) NULL,
                        [ConfiguredUnitCount] [int] NULL,
                        [ContentLastUpdatedAt] [datetime] NULL,
                        [InstalledContentSize] [bigint] NULL,
                        [DeviceCreated] [datetime2](7) NULL,
                        [WifiBSSID] [char](17) NULL,
                        [WifiSSID] [nvarchar](32) NULL,
                        [AccessPointCreated] [datetime2](7) NULL,
                        [UserID] [uniqueidentifier] NULL,
                        [Username] BINARY (256) NULL,
                        [UserType] BINARY (256) NULL,
                        [UserCreated] [datetime2](7) NULL,
                        [SchoolID] [uniqueidentifier] NULL,
                        [DistrictID] [uniqueidentifier] NULL,
                        [LicenseRequestTypeID] [int] NULL
                    )

                    INSERT INTO @tmpLicense
                    SELECT lr.[LicenseRequestID] -- 0
                          ,lr.[ConfigCode] -- 1
                          ,lr.[RequestDateTime] -- 2
                          ,lr.[LocationId] -- 3
                          ,lr.[LocationName] -- 4
                          ,lr.[LearningContentQueued] -- 5
                          ,lr.[Created] LicenseRequestCreated -- 6
                          ,l.[LicenseIssueDateTime] -- 7
                          ,l.[LicenseExpiryDateTime] -- 8
                          ,l.[Created] LicenseCreated -- 9
                          ,d.[DeviceID] -- 10
                          ,d.[DeviceName] -- 11
                          ,d.[DeviceType] -- 12
                          ,d.[DeviceOS] -- 13
                          ,d.[DeviceOSVersion] -- 14
                          ,d.[PSoCAppID] -- 15
                          ,d.[PSoCAppVersion] -- 16
                          ,d.[DeviceFreeSpace] -- 17
                          ,d.[ConfiguredGrades] -- 18
                          ,d.[ConfiguredUnitCount] -- 19
                          ,d.[ContentLastUpdatedAt] -- 20
                          ,d.[InstalledContentSize] --21
                          ,d.[Created] DeviceCreated -- 22
                          ,a.[WifiBSSID] -- 23
                          ,a.[WifiSSID] -- 24
                          ,a.[Created] AccessPointCreated -- 25
                          ,u.[UserID] -- 26
                          ,u.[Username] -- 27
                          ,u.[UserType] -- 28
                          ,u.[Created] UserCreated -- 29
                          ,d.[SchoolID] -- 30
                          ,d.[DistrictID] -- 31
                          ,lr.[LicenseRequestTypeID] -- 32
                      FROM [dbo].[LicenseRequest] lr
                      LEFT JOIN [dbo].[License] l
                        ON lr.[LicenseRequestID] = l.[LicenseRequestID]
                      LEFT JOIN [dbo].[Device] d
                        ON lr.[DeviceID] = d.[DeviceID]
                      LEFT JOIN [dbo].[AccessPoint] a
                        ON lr.[WifiBSSID] = a.[WifiBSSID]
                      LEFT JOIN [dbo].[User] u
                        ON lr.[UserID]= u.[UserID]"
                   + Environment.NewLine
                   + whereClause
                   + Environment.NewLine
                   + (loadNestedTypes ? DeviceInstalledCourseQuery.GetSelectQuery(loadCourse: false, whereClause: "WHERE di.[DeviceID] in (SELECT DeviceID FROM @tmpLicense)") : "")
                   + Environment.NewLine
                   + @"SELECT [LicenseRequestID]
                            ,[ConfigCode]
                            ,[RequestDateTime]
                            ,[LocationId]
                            ,[LocationName]
                            ,[LearningContentQueued]
                            ,[LicenseRequestCreated]
                            ,[LicenseIssueDateTime]
                            ,[LicenseExpiryDateTime]
                            ,[LicenseCreated]
                            ,[DeviceID]
                            ,[DeviceName]
                            ,[DeviceType]
                            ,[DeviceOS]
                            ,[DeviceOSVersion]
                            ,[PSoCAppID]
                            ,[PSoCAppVersion]
                            ,[DeviceFreeSpace]
                            ,[ConfiguredGrades]
                            ,[ConfiguredUnitCount]
                            ,[ContentLastUpdatedAt]
                            ,[InstalledContentSize]
                            ,[DeviceCreated]
                            ,[WifiBSSID]
                            ,[WifiSSID]
                            ,[AccessPointCreated]
                            ,[UserID]
                            ,[Username]
                            ,[UserType]
                            ,[UserCreated]
                            ,[SchoolID]
                            ,[DistrictID]
                            ,[LicenseRequestTypeID]
                           FROM @tmpLicense"
                + Environment.NewLine;

            return new QueryObject { QueryString = query, SqlParameters = parameters };
        }

        public QueryObject GetUpdateQuery(LicenseRequestDto entity)
        {
            return GetInsertQuery(entity, true);
        }

        private QueryObject GetInsertQuery(LicenseRequestDto entity, bool merged)
        {
            var deviceQuery = new DeviceQuery().GetUpdateQuery(entity.Device);

            var licenseInsertQuery = entity.License == null
                ? new QueryObject() : new LicenseQuery().GetInsertQuery(entity.License);

            var parameters = new List<SqlParameter>();
            parameters.AddRange(GetParameterList(entity));

            parameters.AddRange(deviceQuery.SqlParameters
                .Where(p => !parameters.Any(p2 => p.ParameterName.Equals(p2.ParameterName, StringComparison.CurrentCultureIgnoreCase))));

            if (licenseInsertQuery.SqlParameters != null && licenseInsertQuery.SqlParameters.Count > 0)
                parameters.AddRange(licenseInsertQuery.SqlParameters
                    .Where(p => !parameters.Any(p2 => p.ParameterName.Equals(p2.ParameterName, StringComparison.CurrentCultureIgnoreCase))));

            string query = "BEGIN TRANSACTION" + Environment.NewLine
                 + deviceQuery.QueryString
                 + @"MERGE [dbo].[LicenseRequest] t
                              USING
                                (VALUES
                                  (@LicenseRequestID
                                   ,@DistrictID
                                   ,@SchoolID
                                   ,@ConfigCode
                                   ,@WifiBSSID
                                   ,@LicenseRequestTypeID
                                   ,@DeviceID
                                   ,@UserID
                                   ,@RequestDateTime
                                   ,@Response
                                   ,@ResponseDateTime
                                   ,@LocationId
                                   ,@LocationName
                                   ,@LearningContentQueued))
                                   AS s ([LicenseRequestID]
                                           ,[DistrictID]
                                           ,[SchoolID]
                                           ,[ConfigCode]
                                           ,[WifiBSSID]
                                           ,[LicenseRequestTypeID]
                                           ,[DeviceID]
                                           ,[UserID]
                                           ,[RequestDateTime]
                                           ,[Response]
                                           ,[ResponseDateTime]
                                           ,[LocationId]
                                           ,[LocationName]
                                           ,[LearningContentQueued])
                                 ON t.[LicenseRequestID] = s.[LicenseRequestID]
                             WHEN MATCHED THEN
                                UPDATE SET t.[DistrictID] = s.[DistrictID]
                                            ,t.[SchoolID] = s.[SchoolID]
                                            ,t.[ConfigCode] = s.[ConfigCode]
                                            ,t.[WifiBSSID] = s.[WifiBSSID]
                                            ,t.[LicenseRequestTypeID] = s.[LicenseRequestTypeID]
                                            ,t.[DeviceID] = s.[DeviceID]
                                            ,t.[UserID] = s.[UserID]
                                            ,t.[RequestDateTime] = s.[RequestDateTime]
                                            ,t.[Response] = s.[Response]
                                            ,t.[ResponseDateTime] = s.[ResponseDateTime]
                                            ,t.[LocationId] = s.[LocationId]
                                            ,t.[LocationName] = s.[LocationName]
                                            ,t.[LearningContentQueued] = s.[LearningContentQueued]
                             WHEN NOT MATCHED THEN"  // Add the insert SQL
                + Environment.NewLine
                + GetInsertQueryString(true)
                + Environment.NewLine
                + licenseInsertQuery.QueryString
                + Environment.NewLine
                + "COMMIT TRANSACTION";

            return new QueryObject { QueryString = query, SqlParameters = parameters };
        }

        private string GetInsertQueryString(bool merged)
        {
            string query = "INSERT " + (merged ? "" : "INTO [dbo].[LicenseRequest] ")
                + @"([LicenseRequestID]
                                   ,[DistrictID]
                                   ,[SchoolID]
                                   ,[ConfigCode]
                                   ,[WifiBSSID]
                                   ,[LicenseRequestTypeID]
                                   ,[DeviceID]
                                   ,[UserID]
                                   ,[RequestDateTime]
                                   ,[Response]
                                   ,[ResponseDateTime]
                                   ,[LocationId]
                                   ,[LocationName]
                                   ,[LearningContentQueued]
                                   ,[Created])
                             VALUES
                                   (@LicenseRequestID
                                   ,@DistrictID
                                   ,@SchoolID
                                   ,@ConfigCode
                                   ,@WifiBSSID
                                   ,@LicenseRequestTypeID
                                   ,@DeviceID
                                   ,@UserID
                                   ,@RequestDateTime
                                   ,@Response
                                   ,@ResponseDateTime
                                   ,@LocationId
                                   ,@LocationName
                                   ,@LearningContentQueued
                                   ,SYSUTCDATETIME())" + (merged ? ";" : "");

            return query;
        }
    }
}