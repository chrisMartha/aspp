using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Threading.Tasks;

using PSoC.ManagementService.Core;
using PSoC.ManagementService.Data.Helpers;
using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Security;

namespace PSoC.ManagementService.Data.Repositories
{
    public class AccessPointDeviceStatusRepository : IAccessPointDeviceStatusRepository
    {
        public async Task<Tuple<List<DeviceDto>, int>> GetByAdminTypeAsync(AdminType type, Guid? id, int pageSize, int startIndex)
        {
            Guid guidId = id ?? Guid.Empty;
            if (type != AdminType.GlobalAdmin && guidId == Guid.Empty)
                return new Tuple<List<DeviceDto>, int>(new List<DeviceDto>(), 0);
            if (type != AdminType.GlobalAdmin && type != AdminType.DistrictAdmin && type != AdminType.SchoolAdmin)
                return new Tuple<List<DeviceDto>, int>(new List<DeviceDto>(), 0);
            if (pageSize <= 0) pageSize = 10;   // Set invalid page size to default value (10)
            if (startIndex < 0) startIndex = 0; // Set invalid start index to default start index (0
            return await GetResultAsync(type, guidId, pageSize, startIndex).ConfigureAwait(false);
        }

        private async Task<Tuple<List<DeviceDto>, int>> GetResultAsync(AdminType type, Guid id, int pageSize, int startIndex)
        {
            string innerJoinsAndWhereClause; 

            switch (type)
            {
                //TODO: [Performance]Create index for lr.[WifiBSSID] if there's a performance issue for district/school admin
                case AdminType.DistrictAdmin:
                    innerJoinsAndWhereClause = @"
                        INNER JOIN [dbo].[AccessPoint]  ap  ON lr.[WifiBSSID] = ap.[WifiBSSID]
		                INNER JOIN [dbo].[District]     dis ON ap.[DistrictID] = dis.[DistrictID]
		                WHERE dis.[DistrictID] = @districtId
                    ";
                    break;

                case AdminType.SchoolAdmin:
                    innerJoinsAndWhereClause = @"
                        INNER JOIN [dbo].[AccessPoint]  ap  ON lr.[WifiBSSID] = ap.[WifiBSSID]
		                INNER JOIN [dbo].[School]       s   ON ap.[SchoolID]  = s.[SchoolID]
		                WHERE s.[SchoolID] = @schoolId
                    ";
                    break;

                default:
                    innerJoinsAndWhereClause = string.Empty;
                    break;
            }
            //TODO: [Performance]Create index for #TempTable is we get large amount of records at one time
            string query = String.Format(@"
                CREATE TABLE #TempTable (
		                [LicenseRequestID]		UNIQUEIDENTIFIER,
		                [TotalRows]				INT
                )
                ---------------------------------------------------------- 
                INSERT INTO #TempTable (
		                [LicenseRequestID],
		                [TotalRows]
                )
                SELECT  [LicenseRequestID],
		                COUNT(1) OVER() AS [TotalRows]
                FROM (
	                SELECT
		                lr.[Created],
		                lr.[LicenseRequestID],
		                ROW_NUMBER() OVER(
						                PARTITION BY lr.[DeviceID] 
					                    ORDER BY lr.[Created] DESC,
								                 lr.[LicenseRequestTypeID] ASC
					                 ) [SEQ]
		                FROM [dbo].[LicenseRequest] lr	
                        {0}
	                ) r
                WHERE r.[SEQ] = 1	
                ORDER BY r.[Created] DESC
                OFFSET @startIndex ROWS
                FETCH NEXT @pageSize ROWS ONLY
                ---------------------------------------------------------- 
                DECLARE @utcToday DateTime
                SET @utcToday = GETUTCDATE();
                SELECT  lr.[Created],
                        d.[ContentLastUpdatedAt],
                        dis.[DistrictName],
		                s.[SchoolName],
		                lr.[DeviceID],
		                d.[DeviceName],
		                d.[DeviceType],
		                d.[DeviceOSVersion],
		                u.[Username],
		                u.[UserType],
                        d.[ConfiguredGrades],
                        lr.[LocationName],
		                lr.[WifiBSSID],
		                ap.[WifiSSID],
		                lr.[LicenseRequestTypeID],
                        lr.[LearningContentQueued],
                        [CanRevoke] = 
                        CASE 
                            WHEN     c.[LicenseRequestID] IS NOT NULL 
				                 AND c.[LicenseExpiryDateTime] > @utcToday 
			                THEN 1
                            ELSE 0
                        END,
                        tmp.[TotalRows]
                FROM #TempTable tmp
                    INNER JOIN  [dbo].[LicenseRequest]  lr  ON tmp.[licenseRequestID] = lr.[LicenseRequestID]
	                LEFT JOIN   [dbo].[Device]          d   ON lr.[DeviceID] = d.[DeviceID]		
                    LEFT JOIN   [dbo].[User]            u   ON lr.[UserID] = u.[UserID]
                    LEFT JOIN   [dbo].[License]         c   ON lr.[LicenseRequestID] = c.[LicenseRequestID]
                    LEFT JOIN   [dbo].[AccessPoint]     ap  ON lr.[WifiBSSID] = ap.[WifiBSSID]
                    LEFT JOIN   [dbo].[District]        dis ON ap.[DistrictID] = dis.[DistrictID]
                    LEFT JOIN   [dbo].[School]          s   ON ap.[SchoolID]   = s.[SchoolID]
                ORDER BY lr.[Created] DESC
                ----------------------------------------------------------           
                DROP TABLE #TempTable
            ", innerJoinsAndWhereClause);

            var paramList = new List<SqlParameter>()
            {
                new SqlParameter("@startIndex", SqlDbType.Int) { Value = startIndex },
                new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize }
            };

            if (type == AdminType.DistrictAdmin)
            {
                paramList.Add(new SqlParameter("@districtId", SqlDbType.UniqueIdentifier) { Value = id });
            }
            else if (type == AdminType.SchoolAdmin)
            {
                paramList.Add(new SqlParameter("@schoolId", SqlDbType.UniqueIdentifier) { Value = id });
            }

            var result = new List<DeviceDto>();
            int totalRows = 0;
            bool getTotalRows = false;
            using (var dr = await DataAccessHelper.GetDataReaderAsync(query, paramList).ConfigureAwait(false))
            {
                if (dr.HasRows)
                {
                    while (await dr.ReadAsync().ConfigureAwait(false))
                    {
                        var dto = new DeviceDto
                        {
                            ContentLastUpdatedAt = dr.IsDBNull(1) ? (DateTime?) null : dr.GetDateTime(1),
                            DeviceID = dr.GetGuid(4),
                            DeviceNameEnc = dr.IsDBNull(5) ? null : new EncrypedField<string>((byte[])dr.GetValue(5)),
                            DeviceType = dr.IsDBNull(6) ? null : dr.GetString(6),
                            DeviceOSVersion = dr.IsDBNull(7) ? null : dr.GetString(7),
                            ConfiguredGrades = dr.IsDBNull(10) ? null : dr.GetString(10),
                            LastLicenseRequest = new LicenseRequestDto
                            {
                                User = new UserDto
                                {
                                    UsernameEnc = dr.IsDBNull(8) ? null : new EncrypedField<string>((byte[])dr.GetValue(8)),
                                    UserTypeEnc = dr.IsDBNull(9) ? null : new EncrypedField<string>((byte[])dr.GetValue(9))
                                },
                                AccessPoint = new AccessPointDto
                                {
                                    WifiBSSID = dr.GetString(12),
                                    WifiSSID = dr.GetString(13),
                                    District = dr.IsDBNull(2) ? null : new DistrictDto { DistrictName = dr.GetString(2) },
                                    School = dr.IsDBNull(3) ? null : new SchoolDto { SchoolName = dr.GetString(3) }
                                },
                                LocationName = dr.IsDBNull(11) ? null : dr.GetString(11),
                                LicenseRequestType = (LicenseRequestType)dr.GetInt32(14),
                                LearningContentQueued = dr.IsDBNull(15) ? (int?)null : dr.GetInt32(15),
                                Created = dr.GetDateTime(0)
                            },
                            CanRevoke = dr.GetInt32(16) == 1,
                        };

                        result.Add(dto);

                        if (!getTotalRows)
                        {
                            totalRows = dr.GetInt32(17);
                            getTotalRows = true;
                        }
                    }
                }
            }
            return new Tuple<List<DeviceDto>, int>(result, totalRows);
        }

        public Task<bool> DeleteAsync(Guid key)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Guid[] keys)
        {
            throw new NotImplementedException();
        }

        public Task<IList<DeviceDto>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<DeviceDto> GetByIdAsync(Guid key)
        {
            throw new NotImplementedException();
        }

        public Task<IList<DeviceDto>> GetAsync(Expression<Func<IEnumerable<DeviceDto>, IEnumerable<DeviceDto>>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<DeviceDto> InsertAsync(DeviceDto entity)
        {
            throw new NotImplementedException();
        }

        public Task<DeviceDto> UpdateAsync(DeviceDto entity)
        {
            throw new NotImplementedException();
        }
    }
}
