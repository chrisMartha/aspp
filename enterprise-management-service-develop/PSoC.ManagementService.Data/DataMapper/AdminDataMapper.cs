using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Security;

namespace PSoC.ManagementService.Data.DataMapper
{
    public class AdminDataMapper : IDataMapper<AdminDto>
    {
        public async Task<IList<AdminDto>> ToEntityListAsync(DbDataReader dr, bool loadNestedTypes = false)
        {
            var results = new List<AdminDto>();

            if (dr.HasRows)
            {
                while (await dr.ReadAsync().ConfigureAwait(false))
                {
                    var dto = new AdminDto
                    {
                        // Note SchoolId and districtId are keys ao they
                        // will always be populated and loadNestedTypes is ignored.
                        User = new UserDto
                        {
                            UserID = dr.GetGuid(0),
                            UsernameEnc = dr.IsDBNull(1) ? null : new EncrypedField<string>((byte[])dr.GetValue(1)),
                            UserTypeEnc = dr.IsDBNull(2) ? null : new EncrypedField<string>((byte[])dr.GetValue(2)),
                            Created = dr.GetDateTime(3)
                        },
                        District = dr.IsDBNull(4) ? null : new DistrictDto { DistrictId = dr.GetGuid(4) },
                        School = dr.IsDBNull(5) ? null : new SchoolDto { SchoolID = dr.GetGuid(5) },
                        Active = dr.GetBoolean(6),
                        AdminEmail = dr.IsDBNull(7) ? null : dr.GetString(7),
                        LastLoginDateTime = dr.IsDBNull(8) ? (DateTime?)null : dr.GetDateTime(8),
                        Created = dr.GetDateTime(9)
                    };

                    if (!dr.IsDBNull(10))
                        dto.District.DistrictName = dr.GetString(10);

                    if (!dr.IsDBNull(11))
                        dto.School.SchoolName = dr.GetString(11);

                    results.Add(dto);
                }
            }

            return results;
        }
    }
}