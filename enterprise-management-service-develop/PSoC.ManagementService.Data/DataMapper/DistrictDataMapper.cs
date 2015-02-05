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
    public class DistrictDataMapper : IDataMapper<DistrictDto>
    {
        public async Task<IList<DistrictDto>> ToEntityListAsync(DbDataReader dr, bool loadNestedTypes = false)
        {
            var result = new List<DistrictDto>();
            if (dr.HasRows)
            {
                while (await dr.ReadAsync().ConfigureAwait(false))
                {
                    var dto = new DistrictDto
                    {
                        DistrictId = dr.GetGuid(0),
                        CreatedBy = dr.IsDBNull(1) ? null : dr.GetString(1),
                        CreationDate = dr.IsDBNull(2) ? default(DateTime) : dr.GetDateTime(2),
                        DistrictName = dr.GetString(3),
                        DistrictMaxDownloadLicenses = dr.GetInt32(4),
                        DistrictInstructionHoursStart = dr.IsDBNull(5) ? default(TimeSpan?) : dr.GetFieldValue<TimeSpan>(5),
                        DistrictInstructionHoursEnd = dr.IsDBNull(6) ? default(TimeSpan?) : dr.GetFieldValue<TimeSpan>(6),
                        DistrictLicenseExpirySeconds = dr.GetInt32(7),
                        DistrictPreloadHoursStart = dr.IsDBNull(8) ? default(TimeSpan?) : dr.GetFieldValue<TimeSpan>(8),
                        DistrictPreloadHoursEnd = dr.IsDBNull(9) ? default(TimeSpan?) : dr.GetFieldValue<TimeSpan>(9),
                        DistrictOverrideCode = dr.IsDBNull(10) ? null : dr.GetString(10),
                        DistrictUserPolicy = dr.IsDBNull(11) ? default(int?) : dr.GetInt32(11),
                        DistrictUseCacheServer = dr.IsDBNull(12) ? null : dr.GetString(12),
                        DistrictAnnotation = dr.IsDBNull(13) ? null : dr.GetString(13),
                        OAuthApplicationId = dr.GetString(14),
                        OAuthClientId = dr.GetString(15),
                        OAuthURL = dr.GetString(16)
                    };

                    result.Add(dto);
                }
            }

            return result;
        }
    }
}