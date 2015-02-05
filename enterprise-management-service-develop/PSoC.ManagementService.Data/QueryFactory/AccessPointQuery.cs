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
    public class AccessPointQuery : IQueryFactory<AccessPointDto, string>
    {
        public QueryObject GetDeleteManyQuery(string[] keys)
        {
            throw new NotImplementedException();
        }

        public QueryObject GetDeleteQuery(string key)
        {
            string query = @"BEGIN TRANSACTION
                                DELETE FROM [dbo].[License] WHERE [WifiBSSID] = @WifiBSSID
                                DELETE FROM [dbo].[LicenseRequest] WHERE [WifiBSSID] = @WifiBSSID
                                DELETE FROM [dbo].[AccessPoint] WHERE [WifiBSSID] = @WifiBSSID
                            COMMIT TRANSACTION";

            var paramList = new List<SqlParameter>
            {
                new SqlParameter("@WifiBSSID", SqlDbType.Char) { Value =  key},
            };

            return new QueryObject { QueryString = query, SqlParameters = paramList };
        }

        public QueryObject GetInsertQuery(AccessPointDto entity)
        {
            return GetInsertQuery(entity, false);
        }

        public IList<SqlParameter> GetParameterList(AccessPointDto entity)
        {
            Guid districtId =  new Guid();
            Guid schoolId =  new Guid();
            Guid classroomId =  new Guid();

            if (entity.School != null)
            {
                schoolId = entity.School.SchoolID;
                if (entity.School.District != null)
                {
                    districtId = entity.School.District.DistrictId;
                }               
            }
            if (districtId == Guid.Empty && entity.District != null)
            {
                districtId = entity.District.DistrictId;
            }

            if (entity.Classroom != null)
            {
                classroomId = entity.Classroom.ClassroomID;
                if (entity.Classroom.School != null)
                {
                    if (schoolId == Guid.Empty)
                    {
                        schoolId = entity.Classroom.School.SchoolID;
                    }

                    if (districtId == Guid.Empty && entity.Classroom.School.District != null)
                    {
                        districtId = entity.Classroom.School.District.DistrictId;
                    }
                }
            }


            var paramList = new List<SqlParameter>
            {
                new SqlParameter("@WifiBSSID", SqlDbType.Char) { Value =  entity.WifiBSSID},
                new SqlParameter("@WifiSSID", SqlDbType.NVarChar) { Value =  entity.WifiSSID},
                new SqlParameter("@DistrictID", SqlDbType.UniqueIdentifier) { Value = districtId.NullIfEmpty(), IsNullable =true},
                new SqlParameter("@SchoolID", SqlDbType.UniqueIdentifier) { Value =  schoolId.NullIfEmpty(), IsNullable =true},
                new SqlParameter("@ClassroomID", SqlDbType.UniqueIdentifier) { Value = classroomId.NullIfEmpty(), IsNullable =true},
                new SqlParameter("@AccessPointMaxDownloadLicenses", SqlDbType.Int) { Value =  entity.AccessPointMaxDownloadLicenses},
                new SqlParameter("@AccessPointExpiryTimeSeconds", SqlDbType.Int) { Value =  entity.AccessPointExpiryTimeSeconds.NullIfEmpty(), IsNullable =true},
                new SqlParameter("@AccessPointAnnotation", SqlDbType.NVarChar) { Value =  entity.AccessPointAnnotation.NullIfEmpty(), IsNullable =true},
                new SqlParameter("@AccessPointModel", SqlDbType.NVarChar) { Value =  entity.AccessPointModel.NullIfEmpty(), IsNullable =true},

            };

            return paramList;
        }

        public QueryObject GetSelectQuery(string whereClause = "", ICollection<SqlParameter> parameters = null, bool loadNestedTypes = false)
        {
            string query = @"SELECT a.[WifiBSSID]
                              ,a.[WifiSSID]
                              ,a.[DistrictID]
                              ,a.[SchoolID]
                              ,a.[ClassroomID]
                              ,a.[AccessPointMaxDownloadLicenses]
                              ,a.[AccessPointExpiryTimeSeconds]
                              ,a.[AccessPointAnnotation]
                              ,a.[AccessPointModel]
                              ,a.[Created]
                          FROM [dbo].[AccessPoint] a"
                 + Environment.NewLine
                 + whereClause;

            return new QueryObject { QueryString = query, SqlParameters = parameters };
        }

        public QueryObject GetUpdateQuery(AccessPointDto entity)
        {
            var insertQuery = GetInsertQuery(entity, true);
            // if or when a join is needed this will have to change from MERGE
            // to something else. MERGE loses its performace when joins are added.
            string query = @"MERGE [dbo].[AccessPoint] t
                              USING
                                (VALUES
                                  (@WifiBSSID
                                    ,@WifiSSID
                                    ,@DistrictID
                                    ,@SchoolID
                                    ,@ClassroomID
                                    ,@AccessPointMaxDownloadLicenses
                                    ,@AccessPointExpiryTimeSeconds
                                    ,@AccessPointAnnotation
                                    ,@AccessPointModel))
                                   AS s ([WifiBSSID]
                                        ,[WifiSSID]
                                        ,[DistrictID]
                                        ,[SchoolID]
                                        ,[ClassroomID]
                                        ,[AccessPointMaxDownloadLicenses]
                                        ,[AccessPointExpiryTimeSeconds]
                                        ,[AccessPointAnnotation]
                                        ,[AccessPointModel])
                                 ON t.[WifiBSSID] = s.[WifiBSSID]
                             WHEN MATCHED THEN
                                UPDATE SET t.[WifiSSID] = s.[WifiSSID]
                                    ,t.[DistrictID] = s.[DistrictID]
                                    ,t.[SchoolID] = s.[SchoolID]
                                    ,t.[ClassroomID] = s.[ClassroomID]
                                    ,t.[AccessPointMaxDownloadLicenses] = s.[AccessPointMaxDownloadLicenses]
                                    ,t.[AccessPointExpiryTimeSeconds] = s.[AccessPointExpiryTimeSeconds]
                                    ,t.[AccessPointAnnotation] = s.[AccessPointAnnotation]
                                    ,t.[AccessPointModel] = s.[AccessPointModel]
                             WHEN NOT MATCHED THEN"  // Add the insert SQL
                + Environment.NewLine
                + insertQuery.QueryString;

            return new QueryObject { QueryString = query, SqlParameters = insertQuery.SqlParameters };
        }

        private QueryObject GetInsertQuery(AccessPointDto entity, bool merged)
        {
            string query = "INSERT " + (merged ? "" : "INTO [dbo].[AccessPoint] ")
                            + @" ([WifiBSSID]
                                ,[WifiSSID]
                                ,[DistrictID]
                                ,[SchoolID]
                                ,[ClassroomID]
                                ,[AccessPointMaxDownloadLicenses]
                                ,[AccessPointExpiryTimeSeconds]
                                ,[AccessPointAnnotation]
                                ,[AccessPointModel]
                                ,[Created])
                            VALUES
                                (@WifiBSSID
                                ,@WifiSSID
                                ,@DistrictID
                                ,@SchoolID
                                ,@ClassroomID
                                ,@AccessPointMaxDownloadLicenses
                                ,@AccessPointExpiryTimeSeconds
                                ,@AccessPointAnnotation
                                ,@AccessPointModel
                                ,SYSUTCDATETIME())" + (merged ? ";" : "") // <-- needed for merge
                + Environment.NewLine;

            var paramList = GetParameterList(entity);

            return new QueryObject { QueryString = query, SqlParameters = paramList };
        }
    }
}