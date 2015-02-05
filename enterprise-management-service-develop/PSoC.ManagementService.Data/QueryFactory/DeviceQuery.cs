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
    public class DeviceQuery : IQueryFactory<DeviceDto, Guid>
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

            const string query = "DELETE FROM [dbo].[Device] WHERE DeviceID IN (SELECT il.Item FROM @idList il)";

            return new QueryObject { QueryString = query, SqlParameters = paramList };
        }

        public QueryObject GetDeleteQuery(Guid deviceId)
        {
            const string query = @"
                DECLARE @trancount INT;
                SET @trancount = @@trancount;
                BEGIN TRY
                    IF @trancount = 0
                        BEGIN TRANSACTION;
                    ELSE
                        SAVE TRANSACTION DeleteDevice;

                    -- Delete Device record
                    DELETE FROM [dbo].[Device] WHERE DeviceID = @DeviceID;
                    
                lbExit:
                    IF @trancount = 0
                        COMMIT TRANSACTION;
                END TRY
                BEGIN CATCH
                    DECLARE @error INT, @message VARCHAR(4000), @xstate INT;
		            SELECT @error = ERROR_NUMBER(), @message = ERROR_MESSAGE(), @xstate = XACT_STATE();
		            IF @xstate = -1
			            ROLLBACK TRANSACTION;
		            IF @xstate = 1 AND @trancount = 0
			            ROLLBACK TRANSACTION;
		            IF @xstate = 1 AND @trancount > 0
			            ROLLBACK TRANSACTION DeleteAdmin;

		            RAISERROR('DeleteDevice: %d: %s', 16, 1, @error, @message);
                END CATCH";

            var paramList = new List<SqlParameter>
            {
                new SqlParameter("@DeviceID", SqlDbType.UniqueIdentifier) { Value = deviceId }
            };

            return new QueryObject { QueryString = query, SqlParameters = paramList };
        }

        public QueryObject GetInsertQuery(DeviceDto entity)
        {
            return GetInsertQuery(entity, false);
        }

        public IList<SqlParameter> GetParameterList(DeviceDto entity)
        {
            Guid districtId = new Guid();
            Guid schoolId = new Guid();

            if (entity.School != null)
            {
                schoolId = entity.School.SchoolID;
                if (entity.School.District != null)
                {
                    districtId = entity.School.District.DistrictId;
                }
            }
            var paramList = new List<SqlParameter>
            {
                new SqlParameter("@DeviceID", SqlDbType.UniqueIdentifier) { Value =  entity.DeviceID},
                new SqlParameter("@DeviceName", SqlDbType.Binary) { Value =  string.IsNullOrEmpty(entity.DeviceName) ? DBNull.Value : entity.DeviceNameEnc.EncryptedValue.NullIfEmpty(), IsNullable =true},
                new SqlParameter("@DeviceNameHash", SqlDbType.Binary) { Value =  string.IsNullOrEmpty(entity.DeviceName) ? DBNull.Value : entity.DeviceNameEnc.GetHashBytes().NullIfEmpty(), IsNullable =true},
                new SqlParameter("@DeviceType", SqlDbType.NVarChar) { Value =  entity.DeviceType.NullIfEmpty(), IsNullable =true},
                new SqlParameter("@DeviceOS", SqlDbType.NVarChar) { Value =  entity.DeviceOS.NullIfEmpty(), IsNullable =true},
                new SqlParameter("@DeviceOSVersion", SqlDbType.NVarChar) { Value =  entity.DeviceOSVersion.NullIfEmpty(), IsNullable =true},
                new SqlParameter("@PSoCAppID", SqlDbType.UniqueIdentifier) { Value =  entity.PSoCAppID.NullIfEmpty(), IsNullable=true},
                new SqlParameter("@PSoCAppVersion", SqlDbType.NVarChar) { Value =  entity.PSoCAppVersion.NullIfEmpty(), IsNullable =true},
                new SqlParameter("@DeviceFreeSpace", SqlDbType.BigInt) { Value =  entity.DeviceFreeSpace.NullIfEmpty(), IsNullable=true},
                new SqlParameter("@DistrictID", SqlDbType.UniqueIdentifier) { Value = districtId.NullIfEmpty(), IsNullable =true},
                new SqlParameter("@SchoolID", SqlDbType.UniqueIdentifier) { Value =  schoolId.NullIfEmpty(), IsNullable =true},
                new SqlParameter("@LastUsedConfigCode", SqlDbType.NVarChar) { Value =  entity.LastUsedConfigCode.NullIfEmpty(), IsNullable =true},
                new SqlParameter("@ConfiguredGrades", SqlDbType.NVarChar) { Value =  entity.ConfiguredGrades.NullIfEmpty(), IsNullable =true},
                new SqlParameter("@ConfiguredUnitCount", SqlDbType.Int) { Value =  entity.ConfiguredUnitCount.NullIfEmpty(), IsNullable=true},
                new SqlParameter("@ContentLastUpdatedAt", SqlDbType.DateTime) { Value =  entity.ContentLastUpdatedAt.NullIfEmpty(), IsNullable=true},
                new SqlParameter("@InstalledContentSize", SqlDbType.BigInt) { Value =  entity.InstalledContentSize.NullIfEmpty(), IsNullable=true},
                new SqlParameter("@GeoLocation", SqlDbType.NVarChar) { Value =  entity.GeoLocation.NullIfEmpty(), IsNullable =true}
            };

            return paramList;
        }

        public QueryObject GetSelectQuery(string whereClause = "", ICollection<SqlParameter> parameters = null, bool loadNestedTypes = false)
        {
            string query = (loadNestedTypes ?
                            @"DECLARE @tmpDevice Table
                            (
                                [DeviceID] [uniqueidentifier] NOT NULL,
                                [DeviceName] BINARY (256) NULL,
                                [DeviceType] [nvarchar](50) NULL,
                                [DeviceOS] [nvarchar](50) NULL,
                                [DeviceOSVersion] [nvarchar](50) NULL,
                                [PSoCAppID] [uniqueidentifier] NULL,
                                [PSoCAppVersion] [nvarchar](50) NULL,
                                [DeviceFreeSpace] [bigint] NULL,
                                [DistrictID] [uniqueidentifier] NULL,
                                [SchoolID] [uniqueidentifier] NULL,
                                [LastUsedConfigCode] [nvarchar](50) NULL,
                                [DeviceAnnotation] [nvarchar](80) NULL,
                                [ConfiguredGrades] [nvarchar](50) NULL,
                                [ConfiguredUnitCount] [int] NULL,
                                [ContentLastUpdatedAt] [datetime] NULL,
                                [InstalledContentSize] [bigint] NULL,
                                [GeoLocation] [nvarchar](50) NULL,
                                [DeviceCreated] [datetime2](7) NOT NULL
                            )

                            INSERT INTO @tmpDevice" : "")
                          + Environment.NewLine
                          + @"SELECT [DeviceID]
                                ,[DeviceName]
                                ,[DeviceType]
                                ,[DeviceOS]
                                ,[DeviceOSVersion]
                                ,[PSoCAppID]
                                ,[PSoCAppVersion]
                                ,[DeviceFreeSpace]
                                ,[DistrictID]
                                ,[SchoolID]
                                ,[LastUsedConfigCode]
                                ,[DeviceAnnotation]
                                ,[ConfiguredGrades]
                                ,[ConfiguredUnitCount]
                                ,[ContentLastUpdatedAt]
                                ,[InstalledContentSize]
                                ,[GeoLocation]
                                ,[Created]
                            FROM [dbo].[Device]"
                            + Environment.NewLine
                            + whereClause
                            + (loadNestedTypes ? Environment.NewLine
                            + DeviceInstalledCourseQuery.GetSelectQuery(loadCourse: false, whereClause: "WHERE di.[DeviceID] IN (SELECT [DeviceID] FROM @tmpDevice)")
                            + Environment.NewLine
                            + @"SELECT [DeviceID]
                                ,[DeviceName]
                                ,[DeviceType]
                                ,[DeviceOS]
                                ,[DeviceOSVersion]
                                ,[PSoCAppID]
                                ,[PSoCAppVersion]
                                ,[DeviceFreeSpace]
                                ,[DistrictID]
                                ,[SchoolID]
                                ,[LastUsedConfigCode]
                                ,[DeviceAnnotation]
                                ,[ConfiguredGrades]
                                ,[ConfiguredUnitCount]
                                ,[ContentLastUpdatedAt]
                                ,[InstalledContentSize]
                                ,[GeoLocation]
                                ,[DeviceCreated] FROM @tmpDevice" : "");

            return new QueryObject { QueryString = query, SqlParameters = parameters };
        }

        public QueryObject GetUpdateQuery(DeviceDto entity)
        {
            var insertQuery = GetInsertQuery(entity, true);
            // if or when a join is needed this will have to change from MERGE
            // to something else. MERGE loses its performace when joins are added.
            string query = @"MERGE [dbo].[Device] t
                              USING
                                (VALUES
                                  (@DeviceID,
                                    @DeviceName,
                                    @DeviceNameHash,
                                    @DeviceType,
                                    @DeviceOS,
                                    @DeviceOSVersion,
                                    @PSoCAppID,
                                    @PSoCAppVersion,
                                    @DeviceFreeSpace,
                                    @DistrictID,
                                    @SchoolID,
                                    @LastUsedConfigCode,
                                    @ConfiguredGrades,
                                    @ConfiguredUnitCount,
                                    @ContentLastUpdatedAt,
                                    @InstalledContentSize,
                                    @GeoLocation))
                                   AS s ([DeviceID],
                                        [DeviceName],
                                        [DeviceNameHash],
                                        [DeviceType],
                                        [DeviceOS],
                                        [DeviceOSVersion],
                                        [PSoCAppID],
                                        [PSoCAppVersion],
                                        [DeviceFreeSpace],
                                        [DistrictID],
                                        [SchoolID],
                                        [LastUsedConfigCode],
                                        [ConfiguredGrades],
                                        [ConfiguredUnitCount],
                                        [ContentLastUpdatedAt],
                                        [InstalledContentSize],
                                        [GeoLocation])
                                 ON t.[DeviceID] = s.[DeviceID]
                             WHEN MATCHED THEN
                                UPDATE SET t.[DeviceName] = s.[DeviceName]
                                        ,t.[DeviceNameHash] = s.[DeviceNameHash]
                                        ,t.[DeviceType] = s.[DeviceType]
                                        ,t.[DeviceOS] = s.[DeviceOS]
                                        ,t.[DeviceOSVersion] = s.[DeviceOSVersion]
                                        ,t.[PSoCAppID] = s.[PSoCAppID]
                                        ,t.[PSoCAppVersion] = s.[PSoCAppVersion]
                                        ,t.[DeviceFreeSpace] = s.[DeviceFreeSpace]
                                        ,t.[DistrictID] = s.[DistrictID]
                                        ,t.[SchoolID] = s.[SchoolID]
                                        ,t.[LastUsedConfigCode] = s.[LastUsedConfigCode]
                                        ,t.[ConfiguredGrades] = s.[ConfiguredGrades]
                                        ,t.[ConfiguredUnitCount] = s.[ConfiguredUnitCount]
                                        ,t.[ContentLastUpdatedAt] = s.[ContentLastUpdatedAt]
                                        ,t.[InstalledContentSize] = s.[InstalledContentSize]
                                        ,t.[GeoLocation] = s.[GeoLocation]
                             WHEN NOT MATCHED THEN"  // Add the insert SQL
                            + Environment.NewLine
                            + insertQuery.QueryString;

            return new QueryObject { QueryString = query, SqlParameters = insertQuery.SqlParameters };
        }

        private QueryObject GetInsertQuery(DeviceDto entity, bool merged)
        {
            string query = "INSERT " + (merged ? "" : "INTO [dbo].[Device] ")
                            + @"([DeviceID],
                                    [DeviceName],
                                    [DeviceNameHash],
                                    [DeviceType],
                                    [DeviceOS],
                                    [DeviceOSVersion],
                                    [PSoCAppID],
                                    [PSoCAppVersion],
                                    [DeviceFreeSpace],
                                    [DistrictID],
                                    [SchoolID],
                                    [LastUsedConfigCode],
                                    [ConfiguredGrades],
                                    [ConfiguredUnitCount],
                                    [ContentLastUpdatedAt],
                                    [InstalledContentSize],
                                    [GeoLocation],
                                    [Created])
                             VALUES
                                   (@DeviceID,
                                    @DeviceName,
                                    @DeviceNameHash,
                                    @DeviceType,
                                    @DeviceOS,
                                    @DeviceOSVersion,
                                    @PSoCAppID,
                                    @PSoCAppVersion,
                                    @DeviceFreeSpace,
                                    @DistrictID,
                                    @SchoolID,
                                    @LastUsedConfigCode,
                                    @ConfiguredGrades,
                                    @ConfiguredUnitCount,
                                    @ContentLastUpdatedAt,
                                    @InstalledContentSize,
                                    @GeoLocation,
                                    SYSUTCDATETIME())" + (merged ? ";" : "") // <-- needed for merge
                + Environment.NewLine;

            var paramList = new List<SqlParameter>();
            paramList.AddRange(GetParameterList(entity));

            if (entity.DeviceInstalledCourses != null)
            {
                var deviceInstalledQuery = new DeviceInstalledCourseQuery().GetInsertQuery(entity.DeviceInstalledCourses, entity.DeviceID);
                query += deviceInstalledQuery.QueryString + Environment.NewLine;

                paramList.AddRange(deviceInstalledQuery.SqlParameters
                        .Where(p => !paramList.Any(p2 => p.ParameterName.Equals(p2.ParameterName, StringComparison.CurrentCultureIgnoreCase))));
            }

            return new QueryObject { QueryString = query, SqlParameters = paramList };
        }
    }
}