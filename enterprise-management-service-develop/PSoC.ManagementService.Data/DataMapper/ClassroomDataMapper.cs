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
    public class ClassroomDataMapper : IDataMapper<ClassroomDto>
    {
        public Task<IList<ClassroomDto>> ToEntityListAsync(DbDataReader dr, bool loadNestedTypes = false)
        {
            throw new NotImplementedException();
        }
    }
}