using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Security;

namespace PSoC.ManagementService.Data.DataMapper
{
    public class DeviceDataMapper : IDataMapper<DeviceDto>
    {
        public async Task<IList<DeviceDto>> ToEntityListAsync(DbDataReader dr, bool loadNestedTypes = false)
        {
            IList<DeviceInstalledCourseDto> deviceInstalledCourses = new List<DeviceInstalledCourseDto>();
            var results = new List<DeviceDto>();

            if (loadNestedTypes)
            {
                deviceInstalledCourses = (await (new DeviceInstalledCourseMapper()).ToEntityListAsync(dr).ConfigureAwait(false));

                // is there another dataset?
                if (!(await dr.NextResultAsync().ConfigureAwait(false)))
                    return results;
            }

            if (dr.HasRows)
            {
                while (await dr.ReadAsync().ConfigureAwait(false))
                {
                    SchoolDto school = null;
                    DistrictDto district = null;

                    if (!dr.IsDBNull(8))
                    {
                        district = new DistrictDto
                        {
                            DistrictId = dr.GetGuid(8)
                        };
                    }

                    if (!dr.IsDBNull(9))
                    {
                        school = new SchoolDto
                        {
                            SchoolID = dr.GetGuid(9),
                            District = district
                        };
                    }

                    var dto = new DeviceDto
                    {
                        DeviceID = dr.GetGuid(0),
                        DeviceNameEnc = dr.IsDBNull(1) ? null : new EncrypedField<string>((byte[])dr.GetValue(1)),
                        DeviceType = dr.IsDBNull(2) ? null : dr.GetString(2),
                        DeviceOS = dr.IsDBNull(3) ? null : dr.GetString(3),
                        DeviceOSVersion = dr.IsDBNull(4) ? null : dr.GetString(4),
                        PSoCAppID = dr.IsDBNull(5) ? null : dr.GetString(5),
                        PSoCAppVersion = dr.IsDBNull(6) ? null : dr.GetString(6),
                        DeviceFreeSpace = dr.IsDBNull(7) ? default(long?) : dr.GetInt64(7),
                        School = school,
                        LastUsedConfigCode = dr.IsDBNull(10) ? null : dr.GetString(10),
                        DeviceAnnotation = dr.IsDBNull(11) ? null : dr.GetString(11),
                        ConfiguredGrades = dr.IsDBNull(12) ? null : dr.GetString(12),
                        ConfiguredUnitCount = dr.IsDBNull(13) ? default(int?) : dr.GetInt16(13),
                        ContentLastUpdatedAt = dr.IsDBNull(14) ? default(DateTime?) : dr.GetDateTime(14),
                        InstalledContentSize = dr.IsDBNull(15) ? default(long?) : dr.GetInt64(15),
                        GeoLocation = dr.IsDBNull(16) ? null : dr.GetString(16),
                        Created = dr.GetDateTime(17)
                    };

                    if (loadNestedTypes)
                    {
                        dto.DeviceInstalledCourses =
                          deviceInstalledCourses
                          .Where(d => d.Device.DeviceID == dto.DeviceID)
                          .ToArray();
                    }

                    results.Add(dto);
                }
            }

            return results;
        }
    }
}