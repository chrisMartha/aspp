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
    public class UserQuery : IQueryFactory<UserDto, Guid>
    {
        public QueryObject GetDeleteManyQuery(Guid[] keys)
        {
            throw new NotImplementedException();
        }

        public QueryObject GetDeleteQuery(Guid userId)
        {
            return _GetDeleteQuery(userId);
        }

        public QueryObject GetInsertQuery(UserDto entity)
        {
            return _GetInsertQuery(entity, false);
        }

        public static IList<SqlParameter> GetParameterList(UserDto entity)
        {
            var paramList = new List<SqlParameter>
            {
                new SqlParameter("@UserID", SqlDbType.UniqueIdentifier) { Value =  entity.UserID},
                new SqlParameter("@Username", SqlDbType.Binary) { Value =  string.IsNullOrEmpty(entity.Username) ? DBNull.Value : entity.UsernameEnc.EncryptedValue.NullIfEmpty(), IsNullable = true},
                new SqlParameter("@UsernameHash", SqlDbType.Binary) { Value =  string.IsNullOrEmpty(entity.Username) ? DBNull.Value : entity.UsernameEnc.GetHashBytes().NullIfEmpty(), IsNullable =true},
                new SqlParameter("@UserType", SqlDbType.Binary) { Value =  string.IsNullOrEmpty(entity.UserType) ? DBNull.Value : entity.UserTypeEnc.EncryptedValue.NullIfEmpty(), IsNullable = true},
                new SqlParameter("@UserTypeHash", SqlDbType.Binary) { Value =  string.IsNullOrEmpty(entity.UserType) ? DBNull.Value : entity.UserTypeEnc.GetHashBytes().NullIfEmpty(), IsNullable =true},
            };

            return paramList;
        }

        public QueryObject GetSelectQuery(string whereClause = "", ICollection<SqlParameter> parameters = null, bool loadNestedTypes = false)
        {
            string query = @"SELECT [UserID]
                                ,[Username]
                                ,[UserType]
                                ,[Created]
                            FROM [dbo].[User]"
                + Environment.NewLine
                + whereClause;
            return new QueryObject { QueryString = query, SqlParameters = parameters };
        }

        public QueryObject GetUpdateQuery(UserDto entity)
        {
            return _GetUpdateQuery(entity);
        }

        internal static QueryObject _GetDeleteQuery(Guid userId)
        {
            const string query = @"DELETE FROM [dbo].[User] WHERE UserID = @UserID";

            var paramList = new List<SqlParameter>
            {
                new SqlParameter("@UserID", SqlDbType.UniqueIdentifier) { Value = userId },
            };

            return new QueryObject { QueryString = query, SqlParameters = paramList };
        }

        internal static QueryObject _GetUpdateQuery(UserDto entity)
        {            
            // if or when a join is needed this will have to change from MERGE
            // to something else. MERGE loses its performace when joins are added.
            string query = @"UPDATE [dbo].[User]
                                      SET [Username] = @Username
                                         ,[UsernameHash] = @UsernameHash
                                         ,[UserType] = @UserType
                                         ,[UserTypeHash] = @UserTypeHash
                                    WHERE [UserID] = @UserID;"  // Add the insert SQL
                + Environment.NewLine;
            var paramList = GetParameterList(entity);

            return new QueryObject { QueryString = query, SqlParameters = paramList };
        }

        internal static QueryObject _GetInsertQuery(UserDto entity, bool merged)
        {
            string query = "INSERT " + (merged ? "" : "INTO [dbo].[User] ")
                             + @"([UserID]
                                ,[Username]
                                ,[UsernameHash]
                                ,[UserType]
                                ,[UserTypeHash])
                            VALUES
                                (@UserID
                                ,@Username
                                ,@UsernameHash
                                ,@UserType
                                ,@UserTypeHash)" + (merged ? ";" : "") // <-- needed for merge
                + Environment.NewLine;

            var paramList = GetParameterList(entity);

            return new QueryObject { QueryString = query, SqlParameters = paramList };
        }
    }
}