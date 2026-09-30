using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Framework.Repositories.Dapper
{
    public static class DatabaseHelper
    {
        public static TransactionScope GetTransactionScope(IsolationLevel isolationLevel, TimeSpan timeout, TransactionScopeOption scopeOption)
        {
            TransactionOptions transactionOptions = default;
            transactionOptions.IsolationLevel = isolationLevel;
            transactionOptions.Timeout = timeout;
            TransactionOptions transactionOptions2 = transactionOptions;
            return new TransactionScope(scopeOption, transactionOptions2);
        }

        public static TransactionScope GetTransactionScope(IsolationLevel isolationLevel, TimeSpan timeout, TransactionScopeOption scopeOption, TransactionScopeAsyncFlowOption asyncFlowOption)
        {
            TransactionOptions transactionOptions = default;
            transactionOptions.IsolationLevel = isolationLevel;
            transactionOptions.Timeout = timeout;
            TransactionOptions transactionOptions2 = transactionOptions;
            return new TransactionScope(scopeOption, transactionOptions2, asyncFlowOption);
        }
    }
}
