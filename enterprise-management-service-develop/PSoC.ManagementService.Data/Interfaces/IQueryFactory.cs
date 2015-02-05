using System;
using System.Collections.Generic;
using System.Data.SqlClient;

using PSoC.ManagementService.Data.Types;

namespace PSoC.ManagementService.Data.Interfaces
{
    public interface IQueryFactory<TEntity, TKey>
    {
        QueryObject GetDeleteManyQuery(TKey[] keys);

        QueryObject GetDeleteQuery(TKey key);

        QueryObject GetInsertQuery(TEntity entity);

        QueryObject GetSelectQuery(string whereClause = "", ICollection<SqlParameter> parameters = null, bool loadNestedTypes = false);

        QueryObject GetUpdateQuery(TEntity entity);
    }
}