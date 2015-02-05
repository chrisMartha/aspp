using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

using PSoC.ManagementService.Data.DataMapper;
using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Data.QueryFactory;

namespace PSoC.ManagementService.Data.Repositories
{
    public class AccessPointRepository : Repository<AccessPointDto, AccessPointQuery, AccessPointDataMapper, string>, IAccessPointRepository
    {
        public async Task<IList<AccessPointDto>> GetByDistrictAsync(Guid districtId)
        {
            var paramList = new List<SqlParameter>
            {
                new SqlParameter("@DistrictID", SqlDbType.UniqueIdentifier) { Value = districtId}

            };

            var query = GetSelectQuery("WHERE a.DistrictId = @DistrictID", loadNestedTypes: true);
            var result = await GetQueryResultAsync(query.QueryString, paramList, loadNestedTypes: true).ConfigureAwait(false);

            return result;
        }

        public async Task<IList<AccessPointDto>> GetBySchoolAsync(Guid schoolId)
        {
            var paramList = new List<SqlParameter>
            {
                new SqlParameter("@SchoolId", SqlDbType.UniqueIdentifier) { Value = schoolId}

            };

            var query = GetSelectQuery("WHERE a.SchoolId = @SchoolId", loadNestedTypes: true);
            var result = await GetQueryResultAsync(query.QueryString, paramList, loadNestedTypes: true).ConfigureAwait(false);

            return result;
        }
    }
}