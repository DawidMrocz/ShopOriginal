using System.Data.SqlClient;
using System.Data;
using System.Transactions;
using IsolationLevel = System.Transactions.IsolationLevel;

namespace Framework.Repositories.Dapper
{
    public interface IDapperDatabaseService
    {
        Task<IEnumerable<T>> ExecProcedureAsync<T>(string sql, CommandType? commandType = null, TimeSpan? timeout = null, IsolationLevel? isolationLevel = null, TransactionScopeOption? scopeOption = null, params object[] _params);

        Task<IEnumerable<T>> GetListAsync<T>(string sql, CommandType? commandType = null, TimeSpan? timeout = null, IsolationLevel? isolationLevel = null, TransactionScopeOption? scopeOption = null, params object[] _params);

        Task<T?> GetAsync<T>(string sql, CommandType? commandType = null, TimeSpan? timeout = null, IsolationLevel? isolationLevel = null, TransactionScopeOption? scopeOption = null, params object[] _params);

        Task<int> CreateAsync(string sql, CommandType? commandType = null, TimeSpan? timeout = null, IsolationLevel? isolationLevel = null, TransactionScopeOption? scopeOption = null, params object[] _params);

        Task<int> UpdateAsync(string sql, CommandType? commandType = null, TimeSpan? timeout = null, IsolationLevel? isolationLevel = null, TransactionScopeOption? scopeOption = null, params object[] _params);

        Task<int> DeleteAsync(string sql, CommandType? commandType = null, TimeSpan? timeout = null, IsolationLevel? isolationLevel = null, TransactionScopeOption? scopeOption = null, params object[] _params);

        Task<T?> GetValueAsync<T>(string sql, CommandType? commandType = null, TimeSpan? timeout = null, IsolationLevel? isolationLevel = null, TransactionScopeOption? scopeOption = null, params object[] _params);
    }
}
