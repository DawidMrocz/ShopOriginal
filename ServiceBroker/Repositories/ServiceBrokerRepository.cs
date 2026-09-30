using Framework.Configuration;
using ServiceBroker.Resources;
using System.Data;
using System.Data.SqlClient;

namespace ServiceBroker.Repositories
{
    public partial class ServiceBrokerRepository : IServiceBrokerRepository
    {
        private readonly SqlConnection _sqlConnection;

        public ServiceBrokerRepository()
        {
            _sqlConnection = new SqlConnection(FrameworkConfiguration.ConnectionString);
        }


        public void InsertErrorMessage(string message)
        {

            if (_sqlConnection.State != ConnectionState.Open)
            {
                _sqlConnection.Open();
            }

            using (SqlTransaction dbTrans = _sqlConnection.BeginTransaction())
            {
                try
                {
                    using (SqlCommand dbCommand = new SqlCommand(Sql.dbo_ServiceBrokerErrorMessage_Insert, _sqlConnection))
                    {
                        dbCommand.Transaction = dbTrans;
                        dbCommand.Parameters.Add("Message", SqlDbType.NVarChar).Value = message;

                        dbCommand.ExecuteNonQuery();
                    }

                    dbTrans.Commit();
                }
                catch (Exception)
                {
                    dbTrans.Rollback();
                    throw;
                }
            }

            _sqlConnection.Close();
        }
    }
}
