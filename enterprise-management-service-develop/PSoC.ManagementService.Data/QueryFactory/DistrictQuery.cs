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
    /// <summary>
    /// District query factory.
    /// </summary>
    public class DistrictQuery : IQueryFactory<DistrictDto, Guid>
    {
        public QueryObject GetDeleteManyQuery(Guid[] keys)
        {
            throw new NotImplementedException();
        }

        public QueryObject GetDeleteQuery(Guid key)
        {
            string query = @"BEGIN TRANSACTION
                                DELETE FROM [dbo].[Classroom] WHERE DistrictID = @DistrictID
                                DELETE FROM [dbo].[School] WHERE DistrictID = @DistrictID
                                DELETE FROM [dbo].[District] WHERE DistrictID = @DistrictID
                            COMMIT TRANSACTION";

            var paramList = new List<SqlParameter>
            {
                new SqlParameter("@DistrictID", SqlDbType.UniqueIdentifier) { Value =  key},
            };

            return new QueryObject { QueryString = query, SqlParameters = paramList };
        }

        public QueryObject GetInsertQuery(DistrictDto entity)
        {
            return GetInsertQuery(entity, false);
        }

        public IList<SqlParameter> GetParameterList(DistrictDto entity)
        {
            var paramList = new List<SqlParameter>
            {
                new SqlParameter("@DistrictID ", SqlDbType.UniqueIdentifier) { Value =  entity.DistrictId },
                new SqlParameter("@CreatedBy", SqlDbType.NVarChar) { Value =  entity.CreatedBy.NullIfEmpty(), IsNullable = true},
                new SqlParameter("@DistrictName", SqlDbType.NVarChar) { Value =  entity.DistrictName},
                new SqlParameter("@DistrictMaxDownloadLicenses", SqlDbType.Int) { Value =  entity.DistrictMaxDownloadLicenses},
                new SqlParameter("@DistrictInstructionHoursStart", SqlDbType.Time) { Value =  entity.DistrictInstructionHoursStart.NullIfEmpty(), IsNullable = true},
                new SqlParameter("@DistrictInstructionHoursEnd", SqlDbType.Time) { Value =  entity.DistrictInstructionHoursEnd.NullIfEmpty(), IsNullable = true},
                new SqlParameter("@DistrictLicenseExpirySeconds", SqlDbType.Int) { Value =  entity.DistrictLicenseExpirySeconds},
                new SqlParameter("@DistrictPreloadHoursStart", SqlDbType.Time) { Value =  entity.DistrictPreloadHoursStart.NullIfEmpty(), IsNullable = true},
                new SqlParameter("@DistrictPreloadHoursEnd", SqlDbType.Time) { Value =  entity.DistrictPreloadHoursEnd.NullIfEmpty(), IsNullable = true},
                new SqlParameter("@DistrictOverrideCode", SqlDbType.NVarChar) { Value =  entity.DistrictOverrideCode.NullIfEmpty(), IsNullable = true},
                new SqlParameter("@DistrictUserPolicy", SqlDbType.Int) { Value =  entity.DistrictUserPolicy.NullIfEmpty(), IsNullable = true},
                new SqlParameter("@DistrictUseCacheServer", SqlDbType.NVarChar) { Value =  entity.DistrictUseCacheServer.NullIfEmpty(), IsNullable = true},
                new SqlParameter("@DistrictAnnotation", SqlDbType.NVarChar) { Value =  entity.DistrictAnnotation.NullIfEmpty(), IsNullable = true},
                new SqlParameter("@OAuthApplicationId", SqlDbType.NVarChar) { Value =  entity.OAuthApplicationId},
                new SqlParameter("@OAuthClientId", SqlDbType.NVarChar) { Value =  entity.OAuthClientId},
                new SqlParameter("@OAuthURL", SqlDbType.NVarChar) { Value =  entity.OAuthURL},
            };

            return paramList;
        }

        public QueryObject GetSelectQuery(string whereClause = "", ICollection<SqlParameter> parameters = null, bool loadNestedTypes = false)
        {
            string query = @"SELECT [DistrictID]
                              ,[CreatedBy]
                              ,[CreationDate]
                              ,[DistrictName]
                              ,[DistrictMaxDownloadLicenses]
                              ,[DistrictInstructionHoursStart]
                              ,[DistrictInstructionHoursEnd]
                              ,[DistrictLicenseExpirySeconds]
                              ,[DistrictPreloadHoursStart]
                              ,[DistrictPreloadHoursEnd]
                              ,[DistrictOverrideCode]
                              ,[DistrictUserPolicy]
                              ,[DistrictUseCacheServer]
                              ,[DistrictAnnotation]
                              ,[OAuthApplicationId]
                              ,[OAuthClientId]
                              ,[OAuthURL]
                          FROM [dbo].[District]"
                + Environment.NewLine
                + whereClause;

            return new QueryObject { QueryString = query, SqlParameters = parameters };
        }

        public QueryObject GetUpdateQuery(DistrictDto entity)
        {
            var insertQuery = GetInsertQuery(entity, true);
            // if or when a join is needed this will have to change from MERGE
            // to something else. MERGE loses its performace when joins are added.
            string query = @"MERGE [dbo].[District] t
                              USING
                                (VALUES
                                    (@DistrictID
                                    ,@DistrictName
                                    ,@DistrictMaxDownloadLicenses
                                    ,@DistrictInstructionHoursStart
                                    ,@DistrictInstructionHoursEnd
                                    ,@DistrictLicenseExpirySeconds
                                    ,@DistrictPreloadHoursStart
                                    ,@DistrictPreloadHoursEnd
                                    ,@DistrictOverrideCode
                                    ,@DistrictUserPolicy
                                    ,@DistrictUseCacheServer
                                    ,@DistrictAnnotation
                                    ,@OAuthApplicationId
                                    ,@OAuthClientId
                                    ,@OAuthURL))
                                   AS s ([DistrictID]
                                    ,[DistrictName]
                                    ,[DistrictMaxDownloadLicenses]
                                    ,[DistrictInstructionHoursStart]
                                    ,[DistrictInstructionHoursEnd]
                                    ,[DistrictLicenseExpirySeconds]
                                    ,[DistrictPreloadHoursStart]
                                    ,[DistrictPreloadHoursEnd]
                                    ,[DistrictOverrideCode]
                                    ,[DistrictUserPolicy]
                                    ,[DistrictUseCacheServer]
                                    ,[DistrictAnnotation]
                                    ,[OAuthApplicationId]
                                    ,[OAuthClientId]
                                    ,[OAuthURL])
                                 ON t.[DistrictID] = s.[DistrictID]
                             WHEN MATCHED THEN
                                UPDATE SET t.[DistrictName] = s.[DistrictName]
                                    ,t.[DistrictMaxDownloadLicenses] = s.[DistrictMaxDownloadLicenses]
                                    ,t.[DistrictInstructionHoursStart] = s.[DistrictInstructionHoursStart]
                                    ,t.[DistrictInstructionHoursEnd] = s.[DistrictInstructionHoursEnd]
                                    ,t.[DistrictLicenseExpirySeconds] = s.[DistrictLicenseExpirySeconds]
                                    ,t.[DistrictPreloadHoursStart] = s.[DistrictPreloadHoursStart]
                                    ,t.[DistrictPreloadHoursEnd] = s.[DistrictPreloadHoursEnd]
                                    ,t.[DistrictOverrideCode] = s.[DistrictOverrideCode]
                                    ,t.[DistrictUserPolicy] = s.[DistrictUserPolicy]
                                    ,t.[DistrictUseCacheServer] = s.[DistrictUseCacheServer]
                                    ,t.[DistrictAnnotation] = s.[DistrictAnnotation]
                                    ,t.[OAuthApplicationId] = s.[OAuthApplicationId]
                                    ,t.[OAuthClientId] = s.[OAuthClientId]
                                    ,t.[OAuthURL] = s.[OAuthURL]
                             WHEN NOT MATCHED THEN"  // Add the insert SQL
                + Environment.NewLine
                + insertQuery.QueryString;

            return new QueryObject { QueryString = query, SqlParameters = insertQuery.SqlParameters };
        }

        private QueryObject GetInsertQuery(DistrictDto entity, bool merged)
        {
            string query = "INSERT " + (merged ? "" : "INTO [dbo].[District] ")
                            + @"([DistrictID]
                                ,[CreatedBy]
                                ,[CreationDate]
                                ,[DistrictName]
                                ,[DistrictMaxDownloadLicenses]
                                ,[DistrictInstructionHoursStart]
                                ,[DistrictInstructionHoursEnd]
                                ,[DistrictLicenseExpirySeconds]
                                ,[DistrictPreloadHoursStart]
                                ,[DistrictPreloadHoursEnd]
                                ,[DistrictOverrideCode]
                                ,[DistrictUserPolicy]
                                ,[DistrictUseCacheServer]
                                ,[DistrictAnnotation]
                                ,[OAuthApplicationId]
                                ,[OAuthClientId]
                                ,[OAuthURL])
                            VALUES
                                (@DistrictID
                                ,@CreatedBy
                                ,SYSUTCDATETIME()
                                ,@DistrictName
                                ,@DistrictMaxDownloadLicenses
                                ,@DistrictInstructionHoursStart
                                ,@DistrictInstructionHoursEnd
                                ,@DistrictLicenseExpirySeconds
                                ,@DistrictPreloadHoursStart
                                ,@DistrictPreloadHoursEnd
                                ,@DistrictOverrideCode
                                ,@DistrictUserPolicy
                                ,@DistrictUseCacheServer
                                ,@DistrictAnnotation
                                ,@OAuthApplicationId
                                ,@OAuthClientId
                                ,@OAuthURL)" + (merged ? ";" : "") // <-- needed for merge
                + Environment.NewLine;

            var paramList = GetParameterList(entity);

            return new QueryObject { QueryString = query, SqlParameters = paramList };
        }
    }
}