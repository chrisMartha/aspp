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

namespace PSoC.ManagementService.Data.DataMapper
{
    public class LicenseDataMapper : IDataMapper<LicenseDto>
    {
        public async Task<IList<LicenseDto>> ToEntityListAsync(DbDataReader dr, bool loadNestedTypes = false)
        {
            var results = new List<LicenseDto>();
            if (dr.HasRows)
            {
                while (await dr.ReadAsync().ConfigureAwait(false))
                {
                    var dto = new LicenseDto
                    {
                        // Note SchoolId and districtId are keys ao they
                        // will always be populated and loadNestedTypes is ignored.
                        LicenseRequest = new LicenseRequestDto() { LicenseRequestID = dr.GetGuid(0) },
                        School = dr.IsDBNull(2) ? null : new SchoolDto()
                        {
                            District = dr.IsDBNull(1) ? null : new DistrictDto { DistrictId = dr.GetGuid(1) },
                            SchoolID = dr.GetGuid(2)
                        },
                        ConfigCode = dr.GetString(3),
                        WifiBSSID = dr.GetString(4),
                        LicenseIssueDateTime = dr.GetDateTime(5),
                        LicenseExpiryDateTime = dr.GetDateTime(6),
                        Created = dr.GetDateTime(7)
                    };

                    results.Add(dto);

                }
            }

            return results;
        }
    }
}