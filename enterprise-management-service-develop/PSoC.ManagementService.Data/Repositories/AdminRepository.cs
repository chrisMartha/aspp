using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using PSoC.ManagementService.Data.DataMapper;
using PSoC.ManagementService.Data.Helpers;
using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Data.QueryFactory;

using PSoC.ManagementService.Security;

namespace PSoC.ManagementService.Data.Repositories
{
    public class AdminRepository : Repository<AdminDto, AdminQuery, AdminDataMapper, Guid>, IAdminRepository
    {
        public async Task<AdminDto> GetByUsernameAsync(string username)
        {
            AdminDto admin = null;

            const string query = @"
                    SET NOCOUNT ON;

                    SELECT u.[UserID],
                           u.[Username],
                           u.[UserType],
                           u.[Created],
                           a.[DistrictID],
                           a.[SchoolID],
                           a.[Active],
                           a.[AdminEmail],
                           a.[LastLoginDateTime],
                           a.[Created]
                      FROM [dbo].[User] u
                INNER JOIN [dbo].[Admin] a
                        ON u.[UserID] = a.[UserID]
                WHERE u.[UsernameHash] = @UsernameHash";

            var paramList = new List<SqlParameter>
            {
                new SqlParameter("@UsernameHash", SqlDbType.Binary) { Value = Encryption.Instance.ComputeHash(username) }
            };

            using (SqlDataReader dr = await DataAccessHelper.GetDataReaderAsync(query, paramList).ConfigureAwait(false))
            {
                if (dr.HasRows)
                {
                    while (await dr.ReadAsync().ConfigureAwait(false))
                    {
                        var user = new UserDto
                        {
                            UserID = dr.GetGuid(0),
                            UsernameEnc = dr.IsDBNull(1) ? null : new EncrypedField<string>((byte[])dr.GetValue(1)),
                            UserTypeEnc = dr.IsDBNull(2) ? null : new EncrypedField<string>((byte[])dr.GetValue(2)),
                            Created = dr.GetDateTime(3)
                        };
                        var district = dr.IsDBNull(4) ? null : new DistrictDto
                        {
                            DistrictId = dr.GetGuid(4)
                        };
                        var school = dr.IsDBNull(5) ? null : new SchoolDto
                        {
                            SchoolID = dr.GetGuid(5)
                        };
                        admin = new AdminDto
                        {
                            User = user,
                            District = district,
                            School = school,
                            Active = dr.GetBoolean(6),
                            AdminEmail = dr.IsDBNull(7) ? null : dr.GetString(7),
                            LastLoginDateTime = dr.IsDBNull(8) ? (DateTime?) null : dr.GetDateTime(8),
                            Created = dr.GetDateTime(9)
                        };
                    }
                }
            }

            return admin;
        }

        public async Task UpdateLastLoginDateTimeAsync(Guid userId, DateTime loginDateTime)
        {
            const string query = @"
                UPDATE [dbo].[Admin]
                   SET [LastLoginDateTime] = @LoginDateTime
                 WHERE [UserID] = @UserID";

            var paramList = new List<SqlParameter>
            {
                new SqlParameter("@LoginDateTime", SqlDbType.DateTime) { Value = loginDateTime },
                new SqlParameter("@UserID", SqlDbType.UniqueIdentifier) { Value = userId }
            };

            int rowsAffected = await DataAccessHelper.ExecuteAsync(query, paramList).ConfigureAwait(false);
            if (rowsAffected == 0)
            {
                throw new DataException(string.Format("Failed to update last login datetime {0} for user id {1}", loginDateTime, userId));
            }
        }       
    }
}
