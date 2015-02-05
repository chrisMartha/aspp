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
    public class SchoolQuery : IQueryFactory<SchoolDto, Guid>
    {
        public QueryObject GetDeleteManyQuery(Guid[] keys)
        {
            throw new NotImplementedException();
        }

        public QueryObject GetDeleteQuery(Guid key)
        {
            string query = @"BEGIN TRANSACTION
                                DELETE FROM [dbo].[Classroom] WHERE SchoolID = @SchoolID
                                DELETE FROM [dbo].[School] WHERE SchoolID = @SchoolID
                            COMMIT TRANSACTION";

            var paramList = new List<SqlParameter>
            {
                new SqlParameter("@SchoolID", SqlDbType.UniqueIdentifier) { Value =  key},
            };

            return new QueryObject { QueryString = query, SqlParameters = paramList };
        }

        public QueryObject GetInsertQuery(SchoolDto entity)
        {
            return GetInsertQuery(entity, false);
        }

        public IList<SqlParameter> GetParameterList(SchoolDto entity)
        {
            var paramList = new List<SqlParameter>
            {
                new SqlParameter("@SchoolID", SqlDbType.UniqueIdentifier) { Value =  entity.SchoolID},
                new SqlParameter("@DistrictID", SqlDbType.UniqueIdentifier) { Value =  entity.District.DistrictId},
                new SqlParameter("@SchoolName", SqlDbType.NVarChar) { Value =  entity.SchoolName.NullIfEmpty()},
                new SqlParameter("@SchoolAddress1", SqlDbType.NVarChar) { Value =  entity.SchoolAddress1.NullIfEmpty()},
                new SqlParameter("@SchoolAddress2", SqlDbType.NVarChar) { Value =  entity.SchoolAddress2.NullIfEmpty()},
                new SqlParameter("@SchoolTown", SqlDbType.NVarChar) { Value =  entity.SchoolTown.NullIfEmpty()},
                new SqlParameter("@SchoolState", SqlDbType.NVarChar) { Value =  entity.SchoolState.NullIfEmpty()},
                new SqlParameter("@SchoolZipCode", SqlDbType.NVarChar) { Value =  entity.SchoolZipCode.NullIfEmpty()},
                new SqlParameter("@SchoolGrades", SqlDbType.NVarChar) { Value =  entity.SchoolGrades.NullIfEmpty()},
                new SqlParameter("@SchoolMaxDownloadLicenses", SqlDbType.Int) { Value =  entity.SchoolMaxDownloadLicenses},
                new SqlParameter("@SchoolLicenseExpirySeconds", SqlDbType.Int) { Value =  entity.SchoolLicenseExpirySeconds.NullIfEmpty(), IsNullable = true},
                new SqlParameter("@GradeInstructionHoursBegin", SqlDbType.Time) { Value =  entity.GradeInstructionHoursBegin.NullIfEmpty()},
                new SqlParameter("@GradeInstructionHoursEnd", SqlDbType.Time) { Value =  entity.GradeInstructionHoursEnd.NullIfEmpty()},
                new SqlParameter("@SchoolOverrideCode", SqlDbType.NVarChar) { Value =  entity.SchoolOverrideCode.NullIfEmpty()},
                new SqlParameter("@GradePreloadHoursBegin", SqlDbType.Time) { Value =  entity.GradePreloadHoursBegin.NullIfEmpty()},
                new SqlParameter("@GradePreloadHoursEnd", SqlDbType.Time) { Value =  entity.GradePreloadHoursEnd.NullIfEmpty()},
                new SqlParameter("@SchoolUseCacheServer", SqlDbType.NVarChar) { Value =  entity.SchoolUseCacheServer.NullIfEmpty()},
                new SqlParameter("@SchoolUserPolicy", SqlDbType.Int) { Value =  entity.SchoolUserPolicy.NullIfEmpty(), IsNullable = true},
                new SqlParameter("@SchoolAnnotation", SqlDbType.NVarChar) { Value =  entity.SchoolAnnotation.NullIfEmpty()}
            };

            return paramList;
        }

        public QueryObject GetSelectQuery(string whereClause = "", ICollection<SqlParameter> parameters = null, bool loadNestedTypes = false)
        {
            string query = @"SELECT s.[SchoolID]
                            ,s.[SchoolName]
                            ,s.[SchoolAddress1]
                            ,s.[SchoolAddress2]
                            ,s.[SchoolTown]
                            ,s.[SchoolState]
                            ,s.[SchoolZipCode]
                            ,s.[SchoolGrades]
                            ,s.[SchoolMaxDownloadLicenses]
                            ,s.[SchoolLicenseExpirySeconds]
                            ,s.[GradeInstructionHoursBegin]
                            ,s.[GradeInstructionHoursEnd]
                            ,s.[SchoolOverrideCode]
                            ,s.[GradePreloadHoursBegin]
                            ,s.[GradePreloadHoursEnd]
                            ,s.[SchoolUseCacheServer]
                            ,s.[SchoolUserPolicy]
                            ,s.[SchoolAnnotation]
                            ,s.[Created] SchoolCreated
                            ,d.[DistrictID]
                            ,d.[DistrictName]
                            ,d.[DistrictMaxDownloadLicenses]
                            ,d.[DistrictLicenseExpirySeconds]
                            ,d.[OAuthApplicationId]
                            ,d.[OAuthClientId]
                            ,d.[OAuthURL]
                            ,d.[CreationDate] DistrictCreated
                          FROM [dbo].[School] s
                          INNER JOIN [dbo].[District] d
                            ON s.[DistrictID] = d.[DistrictID]"
                + Environment.NewLine
                + whereClause;

            return new QueryObject { QueryString = query, SqlParameters = parameters };
        }

        public QueryObject GetUpdateQuery(SchoolDto entity)
        {
            var insertQuery = GetInsertQuery(entity, true);
            // if or when a join is needed this will have to change from MERGE
            // to something else. MERGE loses its performace when joins are added.
            string query = @"MERGE [dbo].[School] t
                              USING
                                (VALUES
                                 (@SchoolID
                                    ,@DistrictID
                                    ,@SchoolName
                                    ,@SchoolAddress1
                                    ,@SchoolAddress2
                                    ,@SchoolTown
                                    ,@SchoolState
                                    ,@SchoolZipCode
                                    ,@SchoolGrades
                                    ,@SchoolMaxDownloadLicenses
                                    ,@SchoolLicenseExpirySeconds
                                    ,@GradeInstructionHoursBegin
                                    ,@GradeInstructionHoursEnd
                                    ,@SchoolOverrideCode
                                    ,@GradePreloadHoursBegin
                                    ,@GradePreloadHoursEnd
                                    ,@SchoolUseCacheServer
                                    ,@SchoolUserPolicy
                                    ,@SchoolAnnotation))
                                 AS s ([SchoolID]
                                  ,[DistrictID]
                                  ,[SchoolName]
                                  ,[SchoolAddress1]
                                  ,[SchoolAddress2]
                                  ,[SchoolTown]
                                  ,[SchoolState]
                                  ,[SchoolZipCode]
                                  ,[SchoolGrades]
                                  ,[SchoolMaxDownloadLicenses]
                                  ,[SchoolLicenseExpirySeconds]
                                  ,[GradeInstructionHoursBegin]
                                  ,[GradeInstructionHoursEnd]
                                  ,[SchoolOverrideCode]
                                  ,[GradePreloadHoursBegin]
                                  ,[GradePreloadHoursEnd]
                                  ,[SchoolUseCacheServer]
                                  ,[SchoolUserPolicy]
                                  ,[SchoolAnnotation])
                                 ON t.[SchoolID] = s.[SchoolID]
                                WHEN MATCHED THEN
                                   UPDATE SET t.[SchoolID] = s.[SchoolID]
                                    ,t.[DistrictID] = s.[DistrictID]
                                    ,t.[SchoolName] = s.[SchoolName]
                                    ,t.[SchoolAddress1] = s.[SchoolAddress1]
                                    ,t.[SchoolAddress2] = s.[SchoolAddress2]
                                    ,t.[SchoolTown] = s.[SchoolTown]
                                    ,t.[SchoolState] = s.[SchoolState]
                                    ,t.[SchoolZipCode] = s.[SchoolZipCode]
                                    ,t.[SchoolGrades] = s.[SchoolGrades]
                                    ,t.[SchoolMaxDownloadLicenses] = s.[SchoolMaxDownloadLicenses]
                                    ,t.[SchoolLicenseExpirySeconds] = s.[SchoolLicenseExpirySeconds]
                                    ,t.[GradeInstructionHoursBegin] = s.[GradeInstructionHoursBegin]
                                    ,t.[GradeInstructionHoursEnd] = s.[GradeInstructionHoursEnd]
                                    ,t.[SchoolOverrideCode] = s.[SchoolOverrideCode]
                                    ,t.[GradePreloadHoursBegin] = s.[GradePreloadHoursBegin]
                                    ,t.[GradePreloadHoursEnd] = s.[GradePreloadHoursEnd]
                                    ,t.[SchoolUseCacheServer] = s.[SchoolUseCacheServer]
                                    ,t.[SchoolUserPolicy] = s.[SchoolUserPolicy]
                                    ,t.[SchoolAnnotation] = s.[SchoolAnnotation]
                            WHEN NOT MATCHED THEN"  // Add the insert SQL
                            + Environment.NewLine
                            + insertQuery.QueryString;

            return new QueryObject { QueryString = query, SqlParameters = insertQuery.SqlParameters };
        }

        private QueryObject GetInsertQuery(SchoolDto entity, bool merged)
        {
            string query = "INSERT " + (merged ? "" : "INTO [dbo].[School] ")
                              + @"([SchoolID]
                               ,[DistrictID]
                               ,[SchoolName]
                               ,[SchoolAddress1]
                               ,[SchoolAddress2]
                               ,[SchoolTown]
                               ,[SchoolState]
                               ,[SchoolZipCode]
                               ,[SchoolGrades]
                               ,[SchoolMaxDownloadLicenses]
                               ,[SchoolLicenseExpirySeconds]
                               ,[GradeInstructionHoursBegin]
                               ,[GradeInstructionHoursEnd]
                               ,[SchoolOverrideCode]
                               ,[GradePreloadHoursBegin]
                               ,[GradePreloadHoursEnd]
                               ,[SchoolUseCacheServer]
                               ,[SchoolUserPolicy]
                               ,[SchoolAnnotation]
                               ,[Created])
                         VALUES
                               (@SchoolID
                               ,@DistrictID
                               ,@SchoolName
                               ,@SchoolAddress1
                               ,@SchoolAddress2
                               ,@SchoolTown
                               ,@SchoolState
                               ,@SchoolZipCode
                               ,@SchoolGrades
                               ,@SchoolMaxDownloadLicenses
                               ,@SchoolLicenseExpirySeconds
                               ,@GradeInstructionHoursBegin
                               ,@GradeInstructionHoursEnd
                               ,@SchoolOverrideCode
                               ,@GradePreloadHoursBegin
                               ,@GradePreloadHoursEnd
                               ,@SchoolUseCacheServer
                               ,@SchoolUserPolicy
                               ,@SchoolAnnotation
                               ,SYSUTCDATETIME())" + (merged ? ";" : "") // <-- needed for merge
                + Environment.NewLine;

            var paramList = GetParameterList(entity);

            return new QueryObject { QueryString = query, SqlParameters = paramList };
        }
    }
}