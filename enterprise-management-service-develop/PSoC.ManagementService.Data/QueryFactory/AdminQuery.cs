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
    public class AdminQuery : IQueryFactory<AdminDto, Guid>
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

            const string query = "DELETE FROM [dbo].[Admin] WHERE UserID IN (SELECT il.Item FROM @idList il)";

            return new QueryObject { QueryString = query, SqlParameters = paramList };
        }

        public QueryObject GetDeleteQuery(Guid userId)
        {
            QueryObject userDeleteQuery = UserQuery._GetDeleteQuery(userId);

            string query = @"
                DECLARE @trancount INT;
                SET @trancount = @@trancount;
                BEGIN TRY
                    IF @trancount = 0
                        BEGIN TRANSACTION;
                    ELSE
                        SAVE TRANSACTION DeleteAdmin;

                    -- Delete Admin record
                    DELETE FROM [dbo].[Admin] WHERE UserID = @UserID;
                    
                    -- Delete User record
                    " + userDeleteQuery.QueryString + @";

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

		            RAISERROR('DeleteAdmin: %d: %s', 16, 1, @error, @message);
                END CATCH";

            var paramList = new List<SqlParameter>
            {
                new SqlParameter("@UserID", SqlDbType.UniqueIdentifier) { Value = userId }
            };

            return new QueryObject { QueryString = query, SqlParameters = paramList };
        }

        public QueryObject GetInsertQuery(AdminDto entity)
        {
            var userQuery = UserQuery._GetInsertQuery(entity.User, merged: false);

            string query = @"IF NOT EXISTS (SELECT UserID FROM [dbo].[User]
                                               WHERE UserID = @UserID)
                               BEGIN
                                    " + userQuery.QueryString + @"
                               END
                                   IF(@DistrictID IS NULL AND @SchoolID IS NOT NULL)
                                        BEGIN
                                            SET @DistrictID = (SELECT TOP 1 DistrictID FROM [dbo].[School] WHERE SchoolId = @SchoolID)
                                        END
                                   INSERT INTO [dbo].[Admin]
                                              ([UserID]
                                              ,[DistrictID]
                                              ,[SchoolID]
                                              ,[Active]
                                              ,[AdminEmail]
                                              ,[LastLoginDateTime])
                                        VALUES
                                              (@UserID
                                              ,@DistrictID
                                              ,@SchoolID
                                              ,@Active
                                              ,@AdminEmail
                                              ,@LastLoginDateTime);";
            
            var paramList = new List<SqlParameter>();
            paramList.AddRange(GetParameterList(entity));
            
            paramList.AddRange(userQuery.SqlParameters
                    .Where(p => !paramList.Any(p2 => p.ParameterName.Equals(p2.ParameterName, StringComparison.CurrentCultureIgnoreCase))));
            

            return new QueryObject { QueryString = query, SqlParameters = paramList };
        }

        private IList<SqlParameter> GetParameterList(AdminDto entity)
        {
            var paramList = new List<SqlParameter>
            {
                new SqlParameter("@DistrictID", SqlDbType.UniqueIdentifier) { Value = (entity.District == null) ? DBNull.Value : (object) entity.District.DistrictId, IsNullable = true },
                new SqlParameter("@SchoolID", SqlDbType.UniqueIdentifier) { Value = (entity.School == null) ? DBNull.Value : (object) entity.School.SchoolID, IsNullable = true},
                new SqlParameter("@Active", SqlDbType.Bit) { Value = entity.Active },
                new SqlParameter("@AdminEmail", SqlDbType.NVarChar) { Value = (entity.AdminEmail == null) ? DBNull.Value : (object) entity.AdminEmail, IsNullable = true },
                new SqlParameter("@LastLoginDateTime", SqlDbType.DateTime) { Value = (entity.LastLoginDateTime == null) ? DBNull.Value : (object) entity.LastLoginDateTime, IsNullable = true },
            };

            return paramList;
        }

        public QueryObject GetSelectQuery(string whereClause = "", ICollection<SqlParameter> parameters = null, bool loadNestedTypes = false)
        {
            if (!string.IsNullOrWhiteSpace(whereClause))
            {
                int idx = whereClause.IndexOf("UserID =", StringComparison.Ordinal);
                if (idx > 0)
                {
                    whereClause = whereClause.Substring(0, idx) + "a." + whereClause.Substring(idx);
                }
            }

            string query = @"SET NOCOUNT ON;

                             SELECT u.[UserID]
                                   ,u.[Username]
                                   ,u.[UserType]
                                   ,u.[Created]
                                   ,a.[DistrictID]
                                   ,a.[SchoolID]
                                   ,a.[Active]
                                   ,a.[AdminEmail]
                                   ,a.[LastLoginDateTime]
                                   ,a.[Created]
								   ,d.[DistrictName]
								   ,s.[SchoolName]
                               FROM [dbo].[Admin] a
                         INNER JOIN [dbo].[User] u
                                 ON a.[UserID] = u.[UserID]
						 LEFT JOIN [dbo].[District] d
								 ON a.DistrictID = d.DistrictID
						 LEFT JOIN [dbo].[School] s
								 ON a.SchoolID = s.SchoolID"
                + Environment.NewLine
                + whereClause;

            return new QueryObject { QueryString = query, SqlParameters = parameters };
        }

        public QueryObject GetUpdateQuery(AdminDto entity)
        {
            var userQuery = UserQuery._GetUpdateQuery(entity.User);

            string query = @"IF(@DistrictID IS NULL AND @SchoolID IS NOT NULL)
                                        BEGIN
                                            SET @DistrictID = (SELECT TOP 1 DistrictID FROM [dbo].[School] WHERE SchoolId = @SchoolID)
                                        END
                                      " +  userQuery.QueryString + @"
                                      UPDATE [dbo].[Admin]
                                      SET [DistrictID] = @DistrictID
                                         ,[SchoolID] = @SchoolID
                                         ,[Active] = @Active
                                         ,[AdminEmail] = @AdminEmail
                                         ,[LastLoginDateTime] = @LastLoginDateTime
                                    WHERE [UserID] = @UserID";

            var paramList = new List<SqlParameter>();
            paramList.AddRange(GetParameterList(entity));

            paramList.AddRange(userQuery.SqlParameters
                    .Where(p => !paramList.Any(p2 => p.ParameterName.Equals(p2.ParameterName, StringComparison.CurrentCultureIgnoreCase))));


            return new QueryObject { QueryString = query, SqlParameters = paramList };
        }
    }
}