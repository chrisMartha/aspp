using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PSoC.ManagementService.Data.Helpers;
using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Data.Types;

namespace PSoC.ManagementService.Data.QueryFactory
{
    public class DeviceInstalledCourseQuery : IQueryFactory<DeviceInstalledCourseDto, Guid>
    {
        public static string GetSelectQuery(bool loadCourse, string whereClause = "")
        {
            string query = "";
            if(loadCourse)
            {
                query = @"SELECT
                         d.[DeviceID]
                        ,d.[DeviceName]
                        ,d.[DeviceType]
                        ,d.[DeviceOS]
                        ,d.[DeviceOSVersion]
                        ,d.[PSoCAppID]
                        ,d.[PSoCAppVersion]
                        ,d.[DeviceFreeSpace]
                        ,d.[DistrictID]
                        ,d.[SchoolID]
                        ,d.[LastUsedConfigCode]
                        ,d.[DeviceAnnotation]
                        ,d.[ConfiguredGrades]
                        ,d.[ConfiguredUnitCount]
                        ,d.[ContentLastUpdatedAt]
                        ,d.[InstalledContentSize]
                        ,d.[GeoLocation]
                        ,d.[Created] [DeviceCreated]
                        ,di.[CourseLearningResourceID]
                        ,CAST(di.[PercentDownloaded] AS Decimal(38, 20)) [PercentDownloaded]
                        ,di.[LastUpdated]
                        FROM [dbo].[Device] d
                        INNER JOIN [dbo].[DeviceInstalledCourse] di
                            ON d.[DeviceID] = di.[DeviceID]";
            }
            else // load device
            {
                query = @"SELECT di.[DeviceID] -- 0
                          ,CAST(di.[PercentDownloaded] AS Decimal(38, 20)) [PercentDownloaded] -- 1
                          ,di.[LastUpdated] -- 2
                          ,c.[CourseLearningResourceID] -- 3
                          ,c.[CourseName] -- 4
                          ,c.[Grade] -- 5
                          ,c.[Subject] -- 6
                          ,c.[CourseAnnotation] -- 7
                          ,c.[Created] -- 8
                    FROM [dbo].[DeviceInstalledCourse] di
                    INNER JOIN [dbo].[Course] c
                        ON di.[CourseLearningResourceID] = c.[CourseLearningResourceID]";
            }

            query += Environment.NewLine + whereClause;

            return query;
        }

        public QueryObject GetDeleteManyQuery(Guid[] keys)
        {
            throw new NotImplementedException();
        }

        public QueryObject GetDeleteQuery(Guid key)
        {
            throw new NotImplementedException();
        }

        public QueryObject GetInsertQuery(DeviceInstalledCourseDto entity)
        {
            return GetInsertQuery(entity, false);
        }

        public QueryObject GetInsertQuery(ICollection<DeviceInstalledCourseDto> entities, Guid? deviceId = null)
        {
            /*BeginTransaction
             *delete DIC for DeviceID
             * For Each
                 * select C for course_id
                     * if!exist
                         * insert c
                     * end if
                 * insert dic
             * endfor
             * endtransaction
             */
            const string query = @"
                        DELETE FROM [dbo].[DeviceInstalledCourse]
                        WHERE DeviceID = @DeviceId

                        INSERT INTO [dbo].[Course]
                                   ([CourseLearningResourceID]
                                   ,[CourseName]
                                   ,[Grade]
                                   ,[Subject]
                                   ,[CourseAnnotation])
                             SELECT ct.[CourseLearningResourceID]
                                   ,ct.[CourseName]
                                   ,ct.[Grade]
                                   ,ct.[Subject]
                                   ,ct.[CourseAnnotation]
                             FROM @dtCourseTable AS ct
                             WHERE NOT EXISTS (SELECT *
                                              FROM [dbo].[Course] c
                                              WHERE ct.CourseLearningResourceID = c.CourseLearningResourceID)

                        INSERT INTO [dbo].[DeviceInstalledCourse]
                                   ([DeviceID]
                                   ,[CourseLearningResourceID]
                                   ,[PercentDownloaded]
                                   ,[LastUpdated])
                             SELECT dict.[DeviceID]
                                   ,dict.[CourseLearningResourceID]
                                   ,dict.[PercentDownloaded]
                                   ,dict.[LastUpdated]
                             FROM @dtDeviceInstalledCourseTable AS dict";

            var paramList = GetParameterList(entities, deviceId);
            return new QueryObject { QueryString = query, SqlParameters = paramList };
        }

        public IList<SqlParameter> GetParameterList(ICollection<DeviceInstalledCourseDto> entities, Guid? deviceId = null)
        {
            // Build up dtCourseTalbe
            var dtCourseTable = new DataTable();
            dtCourseTable.Columns.Add("CourseLearningResourceID", typeof(Guid));
            dtCourseTable.Columns.Add("CourseName", typeof(string));
            dtCourseTable.Columns.Add("Grade", typeof(string));
            dtCourseTable.Columns.Add("Subject", typeof(string));
            dtCourseTable.Columns.Add("CourseAnnotation", typeof(string));

            foreach (var dic in entities)
            {
                DataRow row = dtCourseTable.NewRow();
                row[0] = dic.Course.CourseLearningResourceID;
                row[1] = dic.Course.CourseName.NullIfEmpty(); //TODO: check what we want for this
                row[2] = dic.Course.Grade.NullIfEmpty();
                row[3] = dic.Course.Subject.NullIfEmpty();
                row[4] = dic.Course.CourseAnnotation.NullIfEmpty(); //TODO: check what we want for this
                dtCourseTable.Rows.Add(row);
            }

            // Build up dtDeviceInstalledCourseTable
            var dtDeviceInstalledCourseTable = new DataTable();
            dtDeviceInstalledCourseTable.Columns.Add("DeviceID", typeof(Guid));
            dtDeviceInstalledCourseTable.Columns.Add("CourseLearningResourceID", typeof(Guid));
            dtDeviceInstalledCourseTable.Columns.Add("PercentDownloaded", typeof(decimal));
            dtDeviceInstalledCourseTable.Columns.Add("LastUpdated", typeof(DateTime));

            foreach (var dic in entities)
            {
                DataRow row = dtDeviceInstalledCourseTable.NewRow();
                row[0] = dic.Device.DeviceID;
                row[1] = dic.Course.CourseLearningResourceID;
                row[2] = dic.PercentDownloaded.NullIfEmpty();
                row[3] = DateTime.UtcNow;
                dtDeviceInstalledCourseTable.Rows.Add(row);
            }

            if(deviceId.HasValue == false)
            {
                deviceId = entities.First().Device.DeviceID;
            }

            var paramList = new List<SqlParameter>
                {
                    new SqlParameter("@DeviceId", SqlDbType.UniqueIdentifier) { Value = deviceId },
                    new SqlParameter("@dtCourseTable", SqlDbType.Structured) { TypeName = "[dbo].[CourseTableType]", Value = dtCourseTable },
                    new SqlParameter("@dtDeviceInstalledCourseTable", SqlDbType.Structured) { TypeName = "[dbo].[DeviceInstalledCourseTableType]", Value = dtDeviceInstalledCourseTable },
                };

            return paramList;
        }

        public IList<SqlParameter> GetParameterList(DeviceInstalledCourseDto entity)
        {
            var paramList = new List<SqlParameter>
            {
                new SqlParameter("@DeviceID", SqlDbType.UniqueIdentifier) { Value =  entity.Device.DeviceID},
                new SqlParameter("@CourseLearningResourceID", SqlDbType.UniqueIdentifier) { Value =  entity.Course.CourseLearningResourceID},
                new SqlParameter("@PercentDownloaded", SqlDbType.Decimal) { Value =  entity.PercentDownloaded.NullIfEmpty(), IsNullable = true},
                new SqlParameter("@LastUpdated", SqlDbType.DateTime) { Value =  entity.LastUpdated.NullIfEmpty(), IsNullable = true}
            };

            return paramList;
        }

        public QueryObject GetSelectQuery(string whereClause = "", ICollection<SqlParameter> parameters = null, bool loadNestedTypes = false)
        {
            throw new NotImplementedException();
        }

        public QueryObject GetUpdateQuery(DeviceInstalledCourseDto entity)
        {
            var insertQuery = GetInsertQuery(entity, true);
            // if or when a join is needed this will have to change from MERGE
            // to something else. MERGE loses its performace when joins are added.
            string query = @"MERGE [dbo].[DeviceInstalledCourse] t
                              USING
                                (VALUES
                                  (@DeviceID
                                     ,@CourseLearningResourceID
                                     ,@PercentDownloaded
                                     ,@LastUpdated))
                                   AS s ([DeviceID]
                                       ,[CourseLearningResourceID]
                                       ,[PercentDownloaded]
                                       ,[LastUpdated])
                                 ON t.[DeviceID] = s.[DeviceID]
                                        AND t.[CourseLearningResourceID] = s.[CourseLearningResourceID]
                             WHEN MATCHED THEN
                                UPDATE SET t.[PercentDownloaded] = s.[PercentDownloaded]
                                    ,t.[LastUpdated] = s.[LastUpdated]
                             WHEN NOT MATCHED THEN"  // Add the insert SQL
                + Environment.NewLine
                + insertQuery.QueryString;

            return new QueryObject { QueryString = query, SqlParameters = insertQuery.SqlParameters };
        }

        private QueryObject GetInsertQuery(DeviceInstalledCourseDto entity, bool merged)
        {
            string query = "INSERT " + (merged ? "" : "INTO [dbo].[DeviceInstalledCourse] ")
                             + @" ([DeviceID]
                                   ,[CourseLearningResourceID]
                                   ,[PercentDownloaded]
                                   ,[LastUpdated])
                            VALUES
                                 (@DeviceID
                                 ,@CourseLearningResourceID
                                 ,@PercentDownloaded
                                 ,@LastUpdated)" + (merged ? ";" : "") // <-- needed for merge
                + Environment.NewLine;

            var paramList = GetParameterList(entity);

            return new QueryObject { QueryString = query, SqlParameters = paramList };
        }
    }
}