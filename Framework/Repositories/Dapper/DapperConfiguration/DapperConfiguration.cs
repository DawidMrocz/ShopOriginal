using Framework.Attribiutes.Dependency;
using Framework.Configuration;
using System.Data;
using System.Transactions;
using IsolationLevel = System.Transactions.IsolationLevel;

namespace Framework.Repositories.Dapper.DapperConfiguration
{
    [DependencyInjection(typeof(IDapperConfiguration))]
    public class DapperConfiguration : IDapperConfiguration
    {
        public string ConnectionString => FrameworkConfiguration.ConnectionString;

        public TimeSpan DefaultTimeout => TransactionManager.DefaultTimeout;

        public IsolationLevel DefaultIsolationLevel => IsolationLevel.ReadUncommitted;

        public TransactionScopeOption DefaultScopeOption => TransactionScopeOption.Required;

        public CommandType DefaultCommandType => CommandType.Text;
    }
}
