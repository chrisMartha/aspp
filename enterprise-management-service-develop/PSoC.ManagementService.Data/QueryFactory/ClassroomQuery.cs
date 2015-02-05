using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Data.Types;

namespace PSoC.ManagementService.Data.QueryFactory
{
    public class ClassroomQuery : IQueryFactory<ClassroomDto, Guid>
    {
        public QueryObject GetDeleteManyQuery(Guid[] keys)
        {
            throw new NotImplementedException();
        }

        public QueryObject GetDeleteQuery(Guid key)
        {
            throw new NotImplementedException();
        }

        public QueryObject GetInsertQuery(ClassroomDto entity)
        {
            throw new NotImplementedException();
        }

        public QueryObject GetSelectQuery(string whereClause = "", ICollection<SqlParameter> parameters = null, bool loadNestedTypes = false)
        {
            throw new NotImplementedException();
        }

        public QueryObject GetUpdateQuery(ClassroomDto entity)
        {
            throw new NotImplementedException();
        }
    }
}