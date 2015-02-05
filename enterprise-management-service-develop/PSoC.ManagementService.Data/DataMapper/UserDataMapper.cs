using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Security;

namespace PSoC.ManagementService.Data.DataMapper
{
    public class UserDataMapper : IDataMapper<UserDto>
    {
        public async Task<IList<UserDto>> ToEntityListAsync(DbDataReader dr, bool loadNestedTypes = false)
        {
            var results = new List<UserDto>();
            if (dr.HasRows)
            {
                while (await dr.ReadAsync().ConfigureAwait(false))
                {
                    var dto = new UserDto
                    {
                        UserID = dr.GetGuid(0),
                        UsernameEnc = dr.IsDBNull(1) ? null : new EncrypedField<string>((byte[])dr.GetValue(1)),
                        UserTypeEnc = dr.IsDBNull(2) ? null : new EncrypedField<string>((byte[])dr.GetValue(2)),
                        Created = dr.GetDateTime(3)
                    };

                    results.Add(dto);
                }
            }

            return results;
        }
    }
}