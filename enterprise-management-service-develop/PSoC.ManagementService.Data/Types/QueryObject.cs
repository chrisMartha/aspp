using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSoC.ManagementService.Data.Types
{
    public class QueryObject
    {
        ICollection<SqlParameter> _sqlParameters = new List<SqlParameter>();

        public string QueryString
        {
            get; set;
        }

        public ICollection<SqlParameter> SqlParameters
        {
            get
            {
                return _sqlParameters;
            }
            set
            {
                _sqlParameters = value;
            }
        }
    }
}