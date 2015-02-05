using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Data.Types;

namespace PSoC.ManagementService.Data.QueryFactory
{
    public class LicenseQuery : IQueryFactory<LicenseDto, Guid>
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

            string query = "DELETE FROM [dbo].[License] WHERE LicenseRequestID IN (SELECT il.Item FROM @idList il)";

            return new QueryObject { QueryString = query, SqlParameters = paramList };
        }

        public QueryObject GetDeleteQuery(Guid key)
        {
            string query = @"DELETE FROM [dbo].[License] WHERE LicenseRequestID = @LicenseRequestID";

            var paramList = new List<SqlParameter>
            {
                new SqlParameter("@LicenseRequestID", SqlDbType.UniqueIdentifier) { Value =  key},
            };

            return new QueryObject { QueryString = query, SqlParameters = paramList };
        }

        public QueryObject GetInsertQuery(LicenseDto entity)
        {
            return GetInsertQuery(entity, false);
        }

        public IList<SqlParameter> GetParameterList(LicenseDto entity)
        {
            var paramList = new List<SqlParameter>
            {
                new SqlParameter("@LicenseRequestID", SqlDbType.UniqueIdentifier) { Value =  entity.LicenseRequest.LicenseRequestID},
                new SqlParameter("@DistrictID", SqlDbType.UniqueIdentifier) { Value = (entity.School == null) ? DBNull.Value : (object) entity.School.District.DistrictId, IsNullable = true },
                new SqlParameter("@SchoolID", SqlDbType.UniqueIdentifier) { Value = (entity.School == null) ? DBNull.Value : (object) entity.School.SchoolID, IsNullable = true },
                new SqlParameter("@ConfigCode", SqlDbType.NVarChar) { Value = entity.ConfigCode },
                new SqlParameter("@WifiBSSID", SqlDbType.Char) { Value = entity.WifiBSSID },
                new SqlParameter("@LicenseIssueDateTime", SqlDbType.DateTime) { Value = entity.LicenseIssueDateTime },
                new SqlParameter("@LicenseExpiryDateTime", SqlDbType.DateTime) { Value = entity.LicenseExpiryDateTime },
            };

            return paramList;
        }

        public QueryObject GetSelectQuery(string whereClause = "", ICollection<SqlParameter> parameters = null, bool loadNestedTypes = false)
        {
            string query = @"SELECT [LicenseRequestID]
                          ,[DistrictID]
                          ,[SchoolID]
                          ,[ConfigCode]
                          ,[WifiBSSID]
                          ,[LicenseIssueDateTime]
                          ,[LicenseExpiryDateTime]
                          ,[Created]
                      FROM [dbo].[License]"
                + Environment.NewLine
                + whereClause;

            return new QueryObject { QueryString = query, SqlParameters = parameters };
        }

        public QueryObject GetUpdateQuery(LicenseDto entity)
        {
            var insertQuery = GetInsertQuery(entity, true);
            // if or when a join is needed this will have to change from MERGE
            // to something else. MERGE loses its performace when joins are added.
            string query = @"MERGE [dbo].[License] t
                              USING
                                (VALUES
                                  (@LicenseRequestID
                                    ,@DistrictID
                                    ,@SchoolID
                                    ,@ConfigCode
                                    ,@WifiBSSID
                                    ,@LicenseIssueDateTime
                                    ,@LicenseExpiryDateTime))
                                   AS s ([LicenseRequestID]
                                        ,[DistrictID]
                                        ,[SchoolID]
                                        ,[ConfigCode]
                                        ,[WifiBSSID]
                                        ,[LicenseIssueDateTime]
                                        ,[LicenseExpiryDateTime])
                                 ON t.[LicenseRequestID] = s.[LicenseRequestID]
                             WHEN MATCHED THEN
                                UPDATE SET t.[LicenseRequestID] = s.[LicenseRequestID]
                                        ,t.[DistrictID] = s.[DistrictID]
                                        ,t.[SchoolID] = s.[SchoolID]
                                        ,t.[ConfigCode] = s.[ConfigCode]
                                        ,t.[WifiBSSID] = s.[WifiBSSID]
                                        ,t.[LicenseIssueDateTime] = s.[LicenseIssueDateTime]
                                        ,t.[LicenseExpiryDateTime] = s.[LicenseExpiryDateTime]
                             WHEN NOT MATCHED THEN"  // Add the insert SQL
                + Environment.NewLine
                + insertQuery.QueryString;

            return new QueryObject { QueryString = query, SqlParameters = insertQuery.SqlParameters };
        }

        private QueryObject GetInsertQuery(LicenseDto entity, bool merged)
        {
            string query =  @"INSERT " + (merged ? "" : "INTO [dbo].[License] ")
                               + @"([LicenseRequestID]
                                ,[DistrictID]
                                ,[SchoolID]
                                ,[ConfigCode]
                                ,[WifiBSSID]
                                ,[LicenseIssueDateTime]
                                ,[LicenseExpiryDateTime]
                                ,[Created])
                            VALUES
                                (@LicenseRequestID
                                ,@DistrictID
                                ,@SchoolID
                                ,@ConfigCode
                                ,@WifiBSSID
                                ,@LicenseIssueDateTime
                                ,@LicenseExpiryDateTime
                                ,SYSUTCDATETIME())" + (merged ? ";" : "")
                + Environment.NewLine;

            var paramList = GetParameterList(entity);

            return new QueryObject { QueryString = query, SqlParameters = paramList };
        }
    }
}