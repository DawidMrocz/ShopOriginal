using Dapper;
using Framework.Attribiutes.Dependency;
using Framework.Repositories.Dapper.DapperConfiguration;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Transactions;
using IsolationLevel = System.Transactions.IsolationLevel;

namespace Framework.Repositories.Dapper
{
    [DependencyInjection(typeof(IDapperDatabaseService))]
    public class DapperDatabaseService : IDapperDatabaseService
    {
        public readonly IDapperConfiguration _configuration;
        public DapperDatabaseService(IDapperConfiguration configuration) => _configuration = configuration;

        public async Task<IEnumerable<T>> ExecProcedureAsync<T>(string sql, CommandType? commandType = null, TimeSpan? timeout = null, IsolationLevel? isolationLevel = null, TransactionScopeOption? scopeOption = null, params object[] parameters)
        {
            using (var transaction = GetTransactionScope(scopeOption, isolationLevel, timeout))
            using (SqlConnection sqlConnection = new(_configuration.ConnectionString))
            {
                sqlConnection.Open();
                var results = await sqlConnection.QueryAsync<T>(sql, AddParameters(parameters), commandType: CommandType.StoredProcedure, commandTimeout: timeout?.Seconds ?? _configuration.DefaultTimeout.Seconds);
                transaction.Complete();
                return results;
            }
        }

        public virtual async Task<IEnumerable<T>> GetListAsync<T>(string sql, CommandType? commandType = null, TimeSpan? timeout = null, IsolationLevel? isolationLevel = null, TransactionScopeOption? scopeOption = null, params object[] parameters)
        {
            using (var transaction = GetTransactionScope(scopeOption, isolationLevel, timeout))
            using (SqlConnection sqlConnection = new(_configuration.ConnectionString))
            {
                sqlConnection.Open();
                var results = await sqlConnection.QueryAsync<T>(sql, AddParameters(parameters), commandType: commandType ?? _configuration.DefaultCommandType, commandTimeout: timeout?.Seconds ?? _configuration.DefaultTimeout.Seconds);
                transaction.Complete();
                return results;
            }
        }

        public virtual async Task<T?> GetAsync<T>(string sql, CommandType? commandType = null, TimeSpan? timeout = null, IsolationLevel? isolationLevel = null, TransactionScopeOption? scopeOption = null, params object[] parameters)
        {
            using (var transaction = GetTransactionScope(scopeOption, isolationLevel, timeout))
            using (SqlConnection sqlConnection = new(_configuration.ConnectionString))
            {
                sqlConnection.Open();
                var results = (await sqlConnection.QueryAsync<T>(sql, AddParameters(parameters), commandType: commandType ?? _configuration.DefaultCommandType, commandTimeout: timeout?.Seconds ?? _configuration.DefaultTimeout.Seconds)).FirstOrDefault();
                transaction.Complete();
                return results;
            }
        }

        public virtual async Task<int> CreateAsync(string sql, CommandType? commandType = null, TimeSpan? timeout = null, IsolationLevel? isolationLevel = null, TransactionScopeOption? scopeOption = null, params object[] parameters)
        {
            using (var transaction = GetTransactionScope(scopeOption, isolationLevel, timeout))
            using (SqlConnection sqlConnection = new(_configuration.ConnectionString))
            {
                sqlConnection.Open();
                var results = await sqlConnection.ExecuteAsync(sql, AddParameters(parameters), commandType: commandType ?? _configuration.DefaultCommandType, commandTimeout: timeout?.Seconds ?? _configuration.DefaultTimeout.Seconds);
                transaction.Complete();
                return results;
            }
        }

        public virtual async Task<int> UpdateAsync(string sql, CommandType? commandType = null, TimeSpan? timeout = null, IsolationLevel? isolationLevel = null, TransactionScopeOption? scopeOption = null, params object[] parameters)
        {
            using (var transaction = GetTransactionScope(scopeOption, isolationLevel, timeout))
            using (SqlConnection sqlConnection = new(_configuration.ConnectionString))
            {
                sqlConnection.Open();
                var results = await sqlConnection.ExecuteAsync(sql, AddParameters(parameters), commandType: commandType ?? _configuration.DefaultCommandType, commandTimeout: timeout?.Seconds ?? _configuration.DefaultTimeout.Seconds);
                transaction.Complete();
                return results;
            }
        }

        public virtual async Task<int> DeleteAsync(string sql, CommandType? commandType = null, TimeSpan? timeout = null, IsolationLevel? isolationLevel = null, TransactionScopeOption? scopeOption = null, params object[] parameters)
        {
            using (var transaction = GetTransactionScope(scopeOption, isolationLevel, timeout))
            using (SqlConnection sqlConnection = new(_configuration.ConnectionString))
            {
                sqlConnection.Open();
                var results = await sqlConnection.ExecuteAsync(sql, AddParameters(parameters), commandType: commandType ?? _configuration.DefaultCommandType, commandTimeout: timeout?.Seconds ?? _configuration.DefaultTimeout.Seconds);
                transaction.Complete();
                return results;
            }
        }

        public virtual async Task<T?> GetValueAsync<T>(string sql, CommandType? commandType = null, TimeSpan? timeout = null, IsolationLevel? isolationLevel = null, TransactionScopeOption? scopeOption = null, params object[] parameters)
        {
            using (var transaction = GetTransactionScope(scopeOption, isolationLevel, timeout))
            using (SqlConnection sqlConnection = new(_configuration.ConnectionString))
            {
                sqlConnection.Open();
                var results = await sqlConnection.ExecuteScalarAsync<T>(sql, AddParameters(parameters), commandType: commandType ?? _configuration.DefaultCommandType, commandTimeout: timeout?.Seconds ?? _configuration.DefaultTimeout.Seconds);
                transaction.Complete();
                return results;
            }
        }

        public virtual TransactionScope GetTransactionScope(TransactionScopeOption? scopeOption, IsolationLevel? isolationLevel, TimeSpan? timeOut)
        {
            return new TransactionScope(scopeOption ?? TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = isolationLevel ?? IsolationLevel.ReadCommitted,
                Timeout = timeOut ?? TimeSpan.FromMinutes(1),
            }, TransactionScopeAsyncFlowOption.Enabled);
        }

        public virtual DynamicParameters AddParameters(params object[] parameters)
        {
            DynamicParameters _params = new();
            foreach (object parameter in parameters)
            {
                _params.AddDynamicParams(parameter);
            }
            return _params;
        }

        private async Task<(DataTable Data, string tmpTableName)> PrepareBulkOperationAsync<T>(SqlConnection connection, string sql, string tableVariableName, IEnumerable<T> tableValues, string? connectionString = null, TimeSpan? timeout = null, IsolationLevel? isolationLevel = null, TransactionScopeOption? scopeOption = null, CommandType? commandType = null, params object[] parameters)
        {
            if (commandType == CommandType.StoredProcedure)
            {
                throw new Exception("Operacjie Bulk nie wspierają stored procedure");
            }

            var tempTableName = "#" + Guid.NewGuid().ToString("N");

            var data = ConvertToDataTable(tableValues);

            await connection.ExecuteAsync(GetCreateTmpTableSql(tempTableName, data), commandType: CommandType.Text);

            sql = sql.Replace(tableVariableName, tempTableName);

            return (data, tempTableName);
        }

        private static DataTable ConvertToDataTable<T>(IEnumerable<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        private string GetCreateTmpTableSql(string tmpTableName, DataTable table)
        {
            var sb = new StringBuilder();
            sb.Append("CREATE TABLE");
            sb.Append(tmpTableName);
            sb.Append('(');
            for (int i = 0; i < table.Columns.Count; i++)
            {
                sb.Append("[");
                sb.Append(table.Columns[i].ColumnName);
                sb.Append(']');
                sb.Append(_sqlTypeConversionMap[table.Columns[i].DataType]);
                if (i != table.Columns.Count - 1)
                {
                    sb.Append(",");
                }
            }
            return sb.ToString();
        }

        private readonly static Dictionary<Type, string> _sqlTypeConversionMap = new Dictionary<Type, string>
        {
            {typeof(string),"NAVCHAR(MAX)" },
            {typeof(short),"INT" },
            {typeof(int),"INT" },
            {typeof(long),"BIGINT" },
            {typeof(decimal),"DECIMAL(18,2)" },
            {typeof(DateTime),"DATETIME" },
            {typeof(TimeSpan),"TIME" },
            {typeof(bool),"BIT" },
            {typeof(Guid),"UNIQUEIDENTIFIER" },
        };
    }
}
