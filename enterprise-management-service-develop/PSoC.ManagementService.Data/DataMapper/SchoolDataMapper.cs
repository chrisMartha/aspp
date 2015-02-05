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
    public class SchoolDataMapper : IDataMapper<SchoolDto>
    {
        public async Task<IList<SchoolDto>> ToEntityListAsync(DbDataReader dr, bool loadNestedTypes = false)
        {
            var results = new List<SchoolDto>();
            if (dr.HasRows)
            {
                while (await dr.ReadAsync().ConfigureAwait(false))
                {
                    var dto = new SchoolDto
                    {
                        //? default(TimeSpan?) : dr.GetFieldValue<TimeSpan>(8),
                        SchoolID = dr.GetGuid(0),
                        SchoolName = dr.IsDBNull(1) ? null : dr.GetString(1),
                        SchoolAddress1 = dr.IsDBNull(2) ? null : dr.GetString(2),
                        SchoolAddress2 = dr.IsDBNull(3) ? null : dr.GetString(3),
                        SchoolTown = dr.IsDBNull(4) ? null : dr.GetString(4),
                        SchoolState = dr.IsDBNull(5) ? null : dr.GetString(5),
                        SchoolZipCode = dr.IsDBNull(6) ? null : dr.GetString(6),
                        SchoolGrades = dr.IsDBNull(7) ? null : dr.GetString(7),
                        SchoolMaxDownloadLicenses = dr.GetInt32(8),
                        SchoolLicenseExpirySeconds = dr.IsDBNull(9) ? default(int?) : dr.GetInt32(9),
                        GradeInstructionHoursBegin = dr.IsDBNull(10) ? default(TimeSpan?) : dr.GetFieldValue<TimeSpan>(10),
                        GradeInstructionHoursEnd = dr.IsDBNull(11) ? default(TimeSpan?) : dr.GetFieldValue<TimeSpan>(11),
                        SchoolOverrideCode = dr.IsDBNull(12) ? null : dr.GetString(12),
                        GradePreloadHoursBegin = dr.IsDBNull(13) ? default(TimeSpan?) : dr.GetFieldValue<TimeSpan>(13),
                        GradePreloadHoursEnd = dr.IsDBNull(14) ? default(TimeSpan?) : dr.GetFieldValue<TimeSpan>(14),
                        SchoolUseCacheServer = dr.IsDBNull(15) ? null : dr.GetString(15),
                        SchoolUserPolicy = dr.IsDBNull(16) ? default(int?) : dr.GetInt32(16),
                        SchoolAnnotation = dr.IsDBNull(17) ? null : dr.GetString(17),
                        Created =  dr.GetDateTime(18),
                        District = new DistrictDto
                        {
                            DistrictId = dr.GetGuid(19),
                            DistrictName = dr.IsDBNull(20) ? null : dr.GetString(20)
                        }

                    };
                    results.Add(dto);
                }
            }

            return results;
        }
    }
}