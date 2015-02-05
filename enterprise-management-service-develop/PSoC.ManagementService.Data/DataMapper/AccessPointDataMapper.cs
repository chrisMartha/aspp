using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Data.Models;

namespace PSoC.ManagementService.Data.DataMapper
{
    public class AccessPointDataMapper : IDataMapper<AccessPointDto>
    {
        public async Task<IList<AccessPointDto>> ToEntityListAsync(DbDataReader dr, bool loadNestedTypes = false)
        {
            var results = new List<AccessPointDto>();
            if (dr.HasRows)
            {
                while (await dr.ReadAsync().ConfigureAwait(false))
                {
                    SchoolDto school = null;
                    ClassroomDto classroom = null;
                    DistrictDto district = null;

                    if (!dr.IsDBNull(2))
                    {
                        district = new DistrictDto
                        {
                            DistrictId = dr.GetGuid(2)
                        };
                    }

                    if (!dr.IsDBNull(3))
                    {
                        school = new SchoolDto
                        {
                            SchoolID = dr.GetGuid(3)
                        };
                    }

                    if (!dr.IsDBNull(4))
                    {
                        classroom = new ClassroomDto
                        {
                            ClassroomID = dr.GetGuid(4),
                        };
                    }

                    var dto = new AccessPointDto
                    {
                        WifiBSSID = dr.GetString(0),
                        WifiSSID = dr.GetString(1),
                        District = district,
                        School = school,
                        Classroom = classroom,
                        AccessPointMaxDownloadLicenses =  dr.GetInt32(5),
                        AccessPointExpiryTimeSeconds = dr.IsDBNull(6) ? default(int?) : dr.GetInt32(6),
                        AccessPointAnnotation = dr.IsDBNull(7) ? null : dr.GetString(7),
                        AccessPointModel = dr.IsDBNull(8) ? null : dr.GetString(8),
                        Created = dr.GetDateTime(9),

                    };
                    results.Add(dto);
                }
            }
            return results;
        }
    }
}