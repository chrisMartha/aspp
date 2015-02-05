using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

using PSoC.ManagementService.Core;
using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Security;

namespace PSoC.ManagementService.Data.DataMapper
{
    public class LicenseRequestDataMapper : IDataMapper<LicenseRequestDto>
    {
        public async Task<IList<LicenseRequestDto>> ToEntityListAsync(DbDataReader dr, bool loadNestedTypes = false)
        {
            var results = new List<LicenseRequestDto>();
            IList<DeviceInstalledCourseDto> deviceInstalledCourses = new List<DeviceInstalledCourseDto>();

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
                    var dto = new LicenseRequestDto
                    {
                        LicenseRequestID = dr.GetGuid(0),
                        ConfigCode = dr.GetString(1),
                        RequestDateTime = dr.GetDateTime(2),
                        LocationId = dr.IsDBNull(3) ? null : dr.GetString(3),
                        LocationName = dr.IsDBNull(4) ? null : dr.GetString(4),
                        LearningContentQueued = dr.IsDBNull(5) ? default(int?) : dr.GetInt32(5),
                        Created = dr.GetDateTime(6),

                        License = dr.IsDBNull(9) ? null : new LicenseDto
                        {
                            LicenseIssueDateTime = dr.GetDateTime(7),
                            LicenseExpiryDateTime = dr.GetDateTime(8),
                            Created = dr.GetDateTime(9)
                        },

                        Device = dr.IsDBNull(10) ? null : new DeviceDto()
                        {
                            DeviceID = dr.GetGuid(10),
                            DeviceNameEnc = dr.IsDBNull(11) ? null : new EncrypedField<string>((byte[])dr.GetValue(11)),
                            DeviceType = dr.IsDBNull(12) ? null : dr.GetString(12),
                            DeviceOS = dr.IsDBNull(13) ? null : dr.GetString(13),
                            DeviceOSVersion = dr.IsDBNull(14) ? null : dr.GetString(14),
                            PSoCAppID = dr.IsDBNull(15) ? null : dr.GetString(15),
                            PSoCAppVersion = dr.IsDBNull(16) ? null : dr.GetString(16),
                            DeviceFreeSpace = dr.IsDBNull(17) ? default(long?) : dr.GetInt64(17),
                            ConfiguredGrades = dr.IsDBNull(18) ? null : dr.GetString(18),
                            ConfiguredUnitCount = dr.IsDBNull(19) ? default(int?) : dr.GetInt32(19),
                            ContentLastUpdatedAt = dr.IsDBNull(20) ? default(DateTime?) : dr.GetDateTime(20),
                            InstalledContentSize = dr.IsDBNull(21) ? default(long?) : dr.GetInt64(21),
                            Created = dr.GetDateTime(22)
                        },

                        AccessPoint = new AccessPointDto
                        {
                            WifiBSSID = dr.GetString(23),
                            WifiSSID = dr.GetString(24),
                            Created = dr.GetDateTime(25)
                        },

                        User = dr.IsDBNull(26) ? null : new UserDto
                        {
                            UserID = dr.GetGuid(26),
                            UsernameEnc = dr.IsDBNull(27) ? null : new EncrypedField<string>((byte[])dr.GetValue(27)),
                            UserTypeEnc = dr.IsDBNull(28) ? null : new EncrypedField<string>((byte[])dr.GetValue(28)),
                            Created = dr.GetDateTime(29)
                        },

                        School = dr.IsDBNull(30) ? null : new SchoolDto
                        {
                            SchoolID = dr.GetGuid(30),
                            District = dr.IsDBNull(31) ? null : new DistrictDto
                            {
                                DistrictId = dr.GetGuid(31)
                            }
                        },

                        LicenseRequestType = (LicenseRequestType)dr.GetInt32(32)

                    };

                    if (loadNestedTypes)
                    {
                        dto.Device.DeviceInstalledCourses =
                            deviceInstalledCourses
                            .Where(d => d.Device.DeviceID == dto.Device.DeviceID)
                            .ToArray();
                    }

                    results.Add(dto);

                }
            }

            return results;
        }
    }
}