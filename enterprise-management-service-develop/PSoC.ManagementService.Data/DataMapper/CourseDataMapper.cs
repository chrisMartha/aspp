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
    public class CourseDataMapper : IDataMapper<CourseDto>
    {
        public async Task<IList<CourseDto>> ToEntityListAsync(DbDataReader dr, bool loadNestedTypes = false)
        {
            IList<DeviceInstalledCourseDto> deviceInstalledCourses = new List<DeviceInstalledCourseDto>();
            var results = new List<CourseDto>();

            if (loadNestedTypes)
            {
                deviceInstalledCourses = (await (new DeviceInstalledCourseMapper()).ToEntityListAsync(dr, false).ConfigureAwait(false));

                // is there another dataset?
                if (!(await dr.NextResultAsync().ConfigureAwait(false)))
                    return results;
            }

            if (dr.HasRows)
            {
                while (await dr.ReadAsync().ConfigureAwait(false))
                {
                    var dto = new CourseDto
                    {
                        CourseLearningResourceID = dr.GetGuid(0),
                        CourseName = dr.IsDBNull(1) ? null : dr.GetString(1),
                        Grade = dr.IsDBNull(2) ? null : dr.GetString(2),
                        Subject = dr.IsDBNull(3) ? null : dr.GetString(3),
                        CourseAnnotation = dr.IsDBNull(4) ? null : dr.GetString(4),
                        Created = dr.GetDateTime(5)
                    };

                    if (loadNestedTypes)
                    {
                        dto.DeviceInstalledCourses =
                              deviceInstalledCourses
                              .Where(d => d.Course.CourseLearningResourceID == dto.CourseLearningResourceID)
                              .ToArray();
                    }

                    results.Add(dto);
                }
            }

            return results;
        }
    }
}