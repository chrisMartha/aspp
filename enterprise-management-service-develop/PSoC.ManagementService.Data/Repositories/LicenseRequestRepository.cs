using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using PSoC.ManagementService.Data.DataMapper;
using PSoC.ManagementService.Data.Helpers;
using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Data.QueryFactory;

namespace PSoC.ManagementService.Data.Repositories
{
    public class LicenseRequestRepository : Repository<LicenseRequestDto, LicenseRequestQuery, LicenseRequestDataMapper, Guid>, ILicenseRequestRepository
    {
        public new async Task<LicenseRequestDto> GetByIdAsync(Guid key)
        {
            var paramList = new List<SqlParameter>
            {
                new SqlParameter("@LicenseRequestID", SqlDbType.UniqueIdentifier) { Value = key}
            };

            var query = GetSelectQuery("WHERE lr.LicenseRequestID = @LicenseRequestID", loadNestedTypes: true);
            var result = await GetQueryResultAsync(query.QueryString, paramList, loadNestedTypes: true).ConfigureAwait(false);

            return result.FirstOrDefault();
        }

        public new async Task<LicenseRequestDto> InsertAsync(LicenseRequestDto entity)
        {
            entity.LicenseRequestID = entity.LicenseRequestID.CreateIfEmpty();

            var query = GetInsertQuery(entity);
            query.QueryString += Environment.NewLine
             + GetSelectQuery("WHERE lr.LicenseRequestID = @LicenseRequestID", loadNestedTypes: true).QueryString;

            var result = await GetQueryResultAsync(query.QueryString, query.SqlParameters, loadNestedTypes: true).ConfigureAwait(false);

            return result.FirstOrDefault();
        }

        public new async Task<LicenseRequestDto> UpdateAsync(LicenseRequestDto entity)
        {
            var query = GetUpdateQuery(entity);
            query.QueryString += Environment.NewLine
             + GetSelectQuery("WHERE lr.LicenseRequestID = @LicenseRequestID", loadNestedTypes: true).QueryString;

            var result = await GetQueryResultAsync(query.QueryString, query.SqlParameters, loadNestedTypes: true).ConfigureAwait(false);

            return result.FirstOrDefault();
        }
    }
}