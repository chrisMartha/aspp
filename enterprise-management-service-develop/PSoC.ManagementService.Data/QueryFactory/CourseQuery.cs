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
    public class CourseQuery : IQueryFactory<CourseDto, Guid>
    {
        public QueryObject GetDeleteManyQuery(Guid[] keys)
        {
            throw new NotImplementedException();
        }

        public QueryObject GetDeleteQuery(Guid key)
        {
            throw new NotImplementedException();
        }

        public QueryObject GetInsertQuery(CourseDto entity)
        {
            return GetInsertQuery(entity, false);
        }

        public IList<SqlParameter> GetParameterList(CourseDto entity)
        {
            var paramList = new List<SqlParameter>
            {
                new SqlParameter("@CourseLearningResourceID", SqlDbType.UniqueIdentifier) { Value =  entity.CourseLearningResourceID},
                new SqlParameter("@CourseName", SqlDbType.NVarChar) { Value =  entity.CourseName.NullIfEmpty(), IsNullable = true},
                new SqlParameter("@Grade", SqlDbType.NVarChar) { Value =  entity.Grade.NullIfEmpty(), IsNullable = true},
                new SqlParameter("@Subject", SqlDbType.NVarChar) { Value =  entity.Subject.NullIfEmpty(), IsNullable = true},
                new SqlParameter("@CourseAnnotation", SqlDbType.NVarChar) { Value =  entity.CourseAnnotation.NullIfEmpty(), IsNullable = true}
            };

            return paramList;
        }

        public QueryObject GetSelectQuery(string whereClause = "", ICollection<SqlParameter> parameters = null, bool loadNestedTypes = false)
        {
            string query = (loadNestedTypes ?
                            @"DECLARE @tmpCourse Table
                            (
                                [CourseLearningResourceID] [uniqueidentifier] NOT NULL,
                                [CourseName] [nvarchar](50) NULL,
                                [Grade] [nvarchar](2) NULL,
                                [Subject] [nvarchar](20) NULL,
                                [CourseAnnotation] [nvarchar](80) NULL,
                                [CourseCreated] [datetime2](7) NOT NULL
                            )
                            INSERT INTO @tmpCourse" : "")
                          + Environment.NewLine
                          + @"SELECT [CourseLearningResourceID]
                                  ,[CourseName]
                                  ,[Grade]
                                  ,[Subject]
                                  ,[CourseAnnotation]
                                  ,[Created]
                              FROM [dbo].[Course]"
                            + Environment.NewLine
                            + whereClause
                            + (loadNestedTypes ? Environment.NewLine
                            + DeviceInstalledCourseQuery.GetSelectQuery(loadCourse: true, whereClause: "WHERE di.[CourseLearningResourceID] IN (SELECT [CourseLearningResourceID] FROM @tmpCourse)")
                            + Environment.NewLine
                            + @"SELECT [CourseLearningResourceID]
                                  ,[CourseName]
                                  ,[Grade]
                                  ,[Subject]
                                  ,[CourseAnnotation]
                                  ,[CourseCreated] FROM @tmpCourse" : "");

            return new QueryObject { QueryString = query, SqlParameters = parameters };
        }

        public QueryObject GetUpdateQuery(CourseDto entity)
        {
            var insertQuery = GetInsertQuery(entity, true);
            // if or when a join is needed this will have to change from MERGE
            // to something else. MERGE loses its performace when joins are added.
            string query = @"MERGE [dbo].[Course] t
                              USING
                                (VALUES
                                  (@CourseLearningResourceID
                                   ,@CourseName
                                   ,@Grade
                                   ,@Subject
                                   ,@CourseAnnotation))
                                   AS s ([CourseLearningResourceID]
                                       ,[CourseName]
                                       ,[Grade]
                                       ,[Subject]
                                       ,[CourseAnnotation])
                                 ON t.[CourseLearningResourceID] = s.[CourseLearningResourceID]
                             WHEN MATCHED THEN
                                UPDATE SET t.[CourseName] = s.[CourseName]
                                        ,t.[Grade] = s.[Grade]
                                        ,t.[Subject] = s.[Subject]
                                        ,t.[CourseAnnotation] = s.[CourseAnnotation]
                             WHEN NOT MATCHED THEN"  // Add the insert SQL
                + Environment.NewLine
                + insertQuery.QueryString;

            return new QueryObject { QueryString = query, SqlParameters = insertQuery.SqlParameters };
        }

        private QueryObject GetInsertQuery(CourseDto entity, bool merged)
        {
            string query = "INSERT " + (merged ? "" : "INTO [dbo].[Course] ")
                             + @"([CourseLearningResourceID]
                                   ,[CourseName]
                                   ,[Grade]
                                   ,[Subject]
                                   ,[CourseAnnotation]
                                   ,[Created])
                            VALUES
                                (@CourseLearningResourceID
                                   ,@CourseName
                                   ,@Grade
                                   ,@Subject
                                   ,@CourseAnnotation
                                ,SYSUTCDATETIME())" + (merged ? ";" : "") // <-- needed for merge
                + Environment.NewLine;

            var paramList = GetParameterList(entity);

            return new QueryObject { QueryString = query, SqlParameters = paramList };
        }
    }
}