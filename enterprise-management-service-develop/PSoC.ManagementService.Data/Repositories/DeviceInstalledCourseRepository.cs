using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using PSoC.ManagementService.Data.Helpers;
using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Data.Models;

namespace PSoC.ManagementService.Data.Repositories
{
    public class DeviceInstalledCourseRepository : IDeviceInstalledCourseRepository
    {
        public Task<bool> DeleteAsync(Tuple<Guid, Guid> key)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Tuple<Guid, Guid>[] keys)
        {
            throw new NotImplementedException();
        }

        public Task<IList<DeviceInstalledCourseDto>> GetAsync(System.Linq.Expressions.Expression<Func<IEnumerable<DeviceInstalledCourseDto>, IEnumerable<DeviceInstalledCourseDto>>> predicate)
        {
            throw new NotImplementedException();
        }

        Task<IList<DeviceInstalledCourseDto>> IDataRepository<DeviceInstalledCourseDto, Tuple<Guid, Guid>>.GetAsync()
        {
            throw new NotImplementedException();
        }

        Task<DeviceInstalledCourseDto> IDataRepository<DeviceInstalledCourseDto, Tuple<Guid, Guid>>.GetByIdAsync(Tuple<Guid, Guid> key)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ImportDataAsync(Guid deviceId, List<DeviceInstalledCourseDto> dicDtos)
        {
            if (dicDtos != null && dicDtos.Count > 0)
            {
                const string query = @"
                BEGIN TRANSACTION
                    DELETE FROM [dbo].[DeviceInstalledCourse]
                    WHERE DeviceID = @DeviceId

                    INSERT INTO [dbo].[Course]
                                ([CourseLearningResourceID]
                                ,[CourseName]
                                ,[Grade]
                                ,[Subject]
                                ,[CourseAnnotation])
                            SELECT ct.[CourseLearningResourceID]
                                ,ct.[CourseName]
                                ,ct.[Grade]
                                ,ct.[Subject]
                                ,ct.[CourseAnnotation]
                            FROM @dtCourseTable AS ct
                            WHERE NOT EXISTS (SELECT *
                                            FROM [dbo].[Course] c
                                            WHERE ct.CourseLearningResourceID = c.CourseLearningResourceID)

                    INSERT INTO [dbo].[DeviceInstalledCourse]
                                ([DeviceID]
                                ,[CourseLearningResourceID]
                                ,[PercentDownloaded]
                                ,[LastUpdated])
                            SELECT dict.[DeviceID]
                                ,dict.[CourseLearningResourceID]
                                ,dict.[PercentDownloaded]
                                ,dict.[LastUpdated]
                            FROM @dtDeviceInstalledCourseTable AS dict

                    SELECT @@rowcount
                COMMIT TRANSACTION";

                // Build up dtCourseTalbe
                var dtCourseTable = new DataTable();
                dtCourseTable.Columns.Add("CourseLearningResourceID", typeof (Guid));
                dtCourseTable.Columns.Add("CourseName", typeof (string));
                dtCourseTable.Columns.Add("Grade", typeof (string));
                dtCourseTable.Columns.Add("Subject", typeof (string));
                dtCourseTable.Columns.Add("CourseAnnotation", typeof (string));

                // Build up dtDeviceInstalledCourseTable
                var dtDeviceInstalledCourseTable = new DataTable();
                dtDeviceInstalledCourseTable.Columns.Add("DeviceID", typeof (Guid));
                dtDeviceInstalledCourseTable.Columns.Add("CourseLearningResourceID", typeof (Guid));
                dtDeviceInstalledCourseTable.Columns.Add("PercentDownloaded", typeof (decimal));
                dtDeviceInstalledCourseTable.Columns.Add("LastUpdated", typeof (DateTime));

                foreach (var dic in dicDtos)
                {
                    if (dic.Course != null)
                    {
                        DataRow rowCourseTable = dtCourseTable.NewRow();
                        rowCourseTable[0] = dic.Course.CourseLearningResourceID;
                        rowCourseTable[1] = (object) dic.Course.CourseName ?? DBNull.Value;//TODO: check what we want for this
                        rowCourseTable[2] = (object) dic.Course.Grade ?? DBNull.Value;
                        rowCourseTable[3] = (object) dic.Course.Subject ?? DBNull.Value;
                        rowCourseTable[4] = (object) dic.Course.CourseAnnotation ?? DBNull.Value;//TODO: check what we want for this
                        dtCourseTable.Rows.Add(rowCourseTable);

                        DataRow rowDeviceInstalledCourseTable = dtDeviceInstalledCourseTable.NewRow();
                        rowDeviceInstalledCourseTable[0] = dic.Device.DeviceID;
                        rowDeviceInstalledCourseTable[1] = dic.Course.CourseLearningResourceID;
                        rowDeviceInstalledCourseTable[2] = (object) dic.PercentDownloaded ?? DBNull.Value;
                        rowDeviceInstalledCourseTable[3] = DateTime.UtcNow;
                        dtDeviceInstalledCourseTable.Rows.Add(rowDeviceInstalledCourseTable);
                    }
                }

                var paramList = new List<SqlParameter>
                {
                    new SqlParameter("@DeviceId", SqlDbType.UniqueIdentifier)
                    {
                        Value = deviceId
                    },
                    new SqlParameter("@dtCourseTable", SqlDbType.Structured)
                    {
                        TypeName = "[dbo].[CourseTableType]",
                        Value = dtCourseTable
                    },
                    new SqlParameter("@dtDeviceInstalledCourseTable", SqlDbType.Structured)
                    {
                        TypeName = "[dbo].[DeviceInstalledCourseTableType]",
                        Value = dtDeviceInstalledCourseTable
                    },
                };
                return (int) await DataAccessHelper.ExecuteScalarAsync(query, paramList).ConfigureAwait(false) > 0;
            }
            else
            {
                const string query = @"
                    DELETE FROM [dbo].[DeviceInstalledCourse]
                    WHERE DeviceID = @DeviceId
                ";

                var paramList = new List<SqlParameter>
                {
                    new SqlParameter("@DeviceId", SqlDbType.UniqueIdentifier)
                    {
                        Value = deviceId
                    }
                };
                int rowsAffected = await DataAccessHelper.ExecuteAsync(query, paramList).ConfigureAwait(false);
                // If there are no rows in DB before, rows affected will be 0
                return rowsAffected >= 0;
            }
        }

        public Task<DeviceInstalledCourseDto> InsertAsync(DeviceInstalledCourseDto entity)
        {
            throw new NotImplementedException();
        }

        public Task<DeviceInstalledCourseDto> UpdateAsync(DeviceInstalledCourseDto entity)
        {
            throw new NotImplementedException();
        }
    }
}
