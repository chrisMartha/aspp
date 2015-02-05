using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PSoC.ManagementService.Data.Helpers
{
    /// <summary>
    /// Data access helper
    /// </summary>
    public static class DataAccessHelper
    {
        /// <summary>
        /// Connection name representing the connection string file.
        /// </summary>
        public const string ConnectionStringName = "PemsConnectionString";

        private static readonly Dictionary<Type, SqlDbType> TypeMap = new Dictionary<Type, SqlDbType>()
        {
                {typeof(bool), SqlDbType.Bit},
                {typeof(bool?), SqlDbType.Bit},
                {typeof(byte), SqlDbType.TinyInt},
                {typeof(byte?), SqlDbType.TinyInt},
                {typeof(byte[]), SqlDbType.Binary},
                {typeof(char), SqlDbType.NChar},
                {typeof(char?), SqlDbType.NChar},
                {typeof(DateTime), SqlDbType.DateTime},
                {typeof(DateTime?), SqlDbType.DateTime},
                {typeof(DateTimeOffset), SqlDbType.DateTimeOffset},
                {typeof(DateTimeOffset?), SqlDbType.DateTimeOffset},
                {typeof(decimal), SqlDbType.Decimal},
                {typeof(decimal?), SqlDbType.Decimal},
                {typeof(double), SqlDbType.Float},
                {typeof(double?), SqlDbType.Float},
                {typeof(float), SqlDbType.Real},
                {typeof(float?), SqlDbType.Real},
                {typeof(Guid), SqlDbType.UniqueIdentifier},
                {typeof(Guid?), SqlDbType.UniqueIdentifier},
                {typeof(int), SqlDbType.Int},
                {typeof(int?), SqlDbType.Int},
                {typeof(long), SqlDbType.BigInt},
                {typeof(long?), SqlDbType.BigInt},
                {typeof(sbyte), SqlDbType.Binary},
                {typeof(sbyte?), SqlDbType.Binary},
                {typeof(short), SqlDbType.SmallInt},
                {typeof(short?), SqlDbType.SmallInt},
                {typeof(string), SqlDbType.NVarChar},
                {typeof(uint), SqlDbType.Int},
                {typeof(uint?), SqlDbType.Int},
                {typeof(ulong), SqlDbType.BigInt},
                {typeof(ulong?), SqlDbType.BigInt},
                {typeof(ushort), SqlDbType.SmallInt},
                {typeof(ushort?), SqlDbType.SmallInt}

        };

        /// <summary>
        /// Global SQL command timeout, default is 30 seconds.
        /// </summary>
        public static Int32 SqlCommandTimeout
        {
            get
            {
                var timeout = ConfigurationManager.AppSettings["Sql.CommandTimeoutInSeconds"];
                return string.IsNullOrEmpty(timeout) ? 30 : Int32.Parse(timeout);
            }
        }

        /// <summary>
        /// Return a new GUID if it's an empty GUID
        /// </summary>
        /// <param name="guid">A GUID</param>
        /// <returns>Existing GUID or a new GUID if it was empty.</returns>
        public static Guid CreateIfEmpty(this Guid guid)
        {
            if (guid.Equals(default(Guid)))
                return Guid.NewGuid();
            return guid;
        }

        /// <summary>
        /// Execute query with a param list
        /// </summary>
        /// <param name="query">The text of the query</param>
        /// <param name="paramList">The Array values to add</param>
        /// <param name="commandType">Value indicating how the CommandText property is to be interpreted</param>
        /// <param name="commandTimeout">The wait time before terminating the attempt to execute a command and generating an error</param>
        /// <param name="database">Database connection string name</param>
        /// <returns>For UPDATE, INSERT, and DELETE statements, the return value is the number of rows affected by the command. For all other types of statements, the return value is -1.</returns>
        public static async Task<int> ExecuteAsync(string query, ICollection<SqlParameter> paramList = null, CommandType commandType = CommandType.Text, int? commandTimeout = null, string database = ConnectionStringName)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings[database].ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (var command = new SqlCommand(query, connection) { CommandType = commandType, CommandTimeout = commandTimeout ?? SqlCommandTimeout })
                {
                    if (paramList != null && paramList.Count > 0) { command.Parameters.AddRange(paramList.ToArray()); }
                    return await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Asyncronously executes the query and returns the first column of the first row in the result set returned by the query.  All other columns and rows are ignored.
        /// </summary>
        /// <param name="query">The text of the query</param>
        /// <param name="paramList">The Array values to add</param>
        /// <param name="commandType">Value indicating how the CommandText property is to be interpreted</param>
        /// <param name="commandTimeout">The wait time before terminating the attempt to execute a command and generating an error</param>
        /// <param name="database">Database connection string name</param>
        /// <returns>First column of the first row in the result set returned by the query. All other columns and rows are ignored.</returns>
        public static async Task<object> ExecuteScalarAsync(string query, ICollection<SqlParameter> paramList = null, CommandType commandType = CommandType.Text, int? commandTimeout = null, string database = ConnectionStringName)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings[database].ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (var command = new SqlCommand(query, connection) { CommandType = commandType, CommandTimeout = commandTimeout ?? SqlCommandTimeout })
                {
                    if (paramList != null && paramList.Count > 0) { command.Parameters.AddRange(paramList.ToArray()); }
                    return await command.ExecuteScalarAsync().ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Execute a datareader with a param list and custom database
        /// </summary>
        /// <param name="query">The text of the query</param>
        /// <param name="paramList">The Array values to add</param>
        /// <param name="commandType">Value indicating how the CommandText property is to be interpreted</param>
        /// <param name="commandTimeout">The wait time before terminating the attempt to execute a command and generating an error</param>
        /// <param name="database">Database connection string name</param>
        /// <returns></returns>
        public static async Task<SqlDataReader> GetDataReaderAsync(string query, ICollection<SqlParameter> paramList = null, CommandType commandType = CommandType.Text, int? commandTimeout = null, string database = ConnectionStringName)
        {
            var connection = new SqlConnection(ConfigurationManager.ConnectionStrings[database].ConnectionString);
            await connection.OpenAsync().ConfigureAwait(false);
            var command = new SqlCommand(query, connection) { CommandType = commandType, CommandTimeout = commandTimeout ?? SqlCommandTimeout };
            if (paramList != null && paramList.Count > 0) { command.Parameters.AddRange(paramList.ToArray()); }
            return await command.ExecuteReaderAsync(CommandBehavior.CloseConnection).ConfigureAwait(false);
        }

        /// <summary>
        /// Fill a datatable with a param list
        /// </summary>
        /// <param name="query">The text of the query</param>
        /// <param name="paramList">The Array values to add</param>
        /// <param name="commandType">Value indicating how the CommandText property is to be interpreted</param>
        /// <param name="commandTimeout">The wait time before terminating the attempt to execute a command and generating an error</param>
        /// <param name="database">Database connection string name</param>
        /// <returns></returns>
        public static async Task<DataTable> GetDataTableAsync(string query, ICollection<SqlParameter> paramList = null, CommandType commandType = CommandType.Text, int? commandTimeout = null, string database = ConnectionStringName)
        {
            var ds = new DataTable();
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings[database].ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (var command = new SqlCommand(query, connection) { CommandType = commandType, CommandTimeout = commandTimeout ?? SqlCommandTimeout })
                {
                    if (paramList != null && paramList.Count > 0) { command.Parameters.AddRange(paramList.ToArray()); }
                    var adapter = new SqlDataAdapter(command);
                    adapter.Fill(ds);
                }
            }
            return ds;
        }

        /// <summary>
        /// Sets value to null of the parameter is of a not nullable type. If is a nullable type the value is passd as is.
        /// </summary>
        /// <typeparam name="T">the Type</typeparam>
        /// <param name="value">the value</param>
        /// <returns>null if defult value; othersize the value for the type.</returns>
        public static object NullIfEmpty<T>(this T value)
        {
            object obj = value;

            if (obj != null)
            {
                if (typeof(T) == typeof(Guid))
                {
                    var g = Guid.Parse(value.ToString());
                    if (g == Guid.Empty)
                        obj = DBNull.Value;
                }
                else if (typeof(T) == typeof(string))
                {
                    var str = value.ToString();
                    if (string.IsNullOrEmpty(str))
                        obj = DBNull.Value;
                }
                else if (typeof(T) == typeof(byte[]))
                {
                    var b = value as byte[];
                    if (b.Length <= 0)
                        obj = DBNull.Value;
                }
            }
            else
            {
                obj = DBNull.Value;
            }

            return obj;
        }

        /// <summary>
        /// Converts an Expression to a partial SQL query string
        /// </summary>
        /// <param name="expression">the expression to convert</param>
        /// <returns>a partial SQL query string</returns>
        public static string ToMSSqlString(this Expression expression)
        {
            ExpressionTranslator qt = new ExpressionTranslator();
            var sql = qt.TranslateToSql(expression);

            return sql;
        }

        /// <summary>
        /// Converts the System.Type to SqlDbType
        /// </summary>
        /// <param name="type">the Syetm.Type</param>
        /// <returns>SqlDbType conversion</returns>
        public static SqlDbType ToSqlDbType(this System.Type type)
        {
            return TypeMap[type];
        }

        /// <summary>
        /// Return a string representing SQL parameters
        /// </summary>
        /// <param name="parameters">SQL parameters</param>
        /// <returns>A message with all parameter names and values.</returns>
        public static String ToString(this IEnumerable<SqlParameter> parameters)
        {
            if (parameters == null)
            {
                return null;
            }

            var sb = new StringBuilder();
            sb.Append("SQL Parameters:\n");
            foreach (var p in parameters)
            {
                switch (p.SqlDbType)
                {
                    // Skip complex types
                    case SqlDbType.Binary:
                    case SqlDbType.Image:
                    case SqlDbType.Structured:
                    case SqlDbType.VarBinary:
                        continue;
                    default:
                        sb.AppendFormat("{0}: {1}", p.ParameterName, p.Value);
                        sb.Append("\n");
                        break;
                }
            }
            return sb.ToString();
        }
    }
}