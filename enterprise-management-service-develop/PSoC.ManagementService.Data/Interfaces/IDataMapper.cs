using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PSoC.ManagementService.Data.Interfaces;

namespace PSoC.ManagementService.Data.Interfaces
{
    public interface IDataMapper<TEntity>
    {
        Task<IList<TEntity>> ToEntityListAsync(DbDataReader dr, bool loadNestedTypes = false);
    }
}