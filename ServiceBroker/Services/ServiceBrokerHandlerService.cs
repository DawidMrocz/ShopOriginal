using Framework.Configuration;
using log4net;
using ServiceBroker.Repositories;
using ServiceBroker.System;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;

namespace ServiceBroker.Services
{
    public class ServiceBrokerHandlerService
    {
        private readonly SqlConnection _sqlConnection;
        private readonly CancellationToken _cancellationToken;
        private readonly IServiceBrokerRepository _serviceBrokerRepository;

        private const string QueueSqlCommand = "WAITFOR (RECEIVE TOP (1) conversation_handle, message_type_name, CAST(message_body AS NVARCHAR(MAX)) FROM {0}), TIMEOUT {1}";

        private const string EndConversationSqlCommand = "End Conversation @handle";
        private const string EndConversationSqlParameter = "@handle";
        private const int ErrorsCountLimit = 10;
        private List<Exception> _exceptions;

        private readonly IEnumerable<Models.MessageHandler> _messageHandlers;

        public readonly ILog Logger = LogManager.GetLogger(typeof(ServiceBrokerHandlerService));

        public ServiceBrokerHandlerService(CancellationToken ct)
        {
            _sqlConnection = new SqlConnection(FrameworkConfiguration.ConnectionString);
            _cancellationToken = ct;
            _messageHandlers = Dependencies.LoadHandlers();
            _serviceBrokerRepository = new ServiceBrokerRepository();
        }

        public void StartListening()
        {
            Logger.Info("Zaczynam nasłuchiwać");
            _cancellationToken.ThrowIfCancellationRequested();

            try
            {
                _sqlConnection.Open();
                var errorsCount = 0;
                string messageToInsert = string.Empty;

                do
                {
                    try
                    {
                        using var tran = _sqlConnection.BeginTransaction();
                        var comm = _sqlConnection.CreateCommand();

                        comm.Transaction = tran;
                        comm.CommandText = string.Format(QueueSqlCommand, FrameworkConfiguration.QueueName, FrameworkConfiguration.QueueTimeout);

                        using (var reader = comm.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var handle = reader.GetSqlGuid(0);
                                var typeName = reader.GetSqlString(1);
                                var message = reader.GetSqlString(2);

                                reader.Close();

                                if (typeName == FrameworkConfiguration.EndDialogMessageType || typeName == FrameworkConfiguration.ErrorMessageType)
                                {
                                    if (typeName == FrameworkConfiguration.ErrorMessageType)
                                    {
                                        Logger.Error(message.ToString());
                                    }

                                    var endconv = _sqlConnection.CreateCommand();
                                    endconv.Transaction = tran;
                                    endconv.CommandText = EndConversationSqlCommand;
                                    endconv.Parameters.Add(EndConversationSqlParameter, SqlDbType.UniqueIdentifier);
                                    endconv.Parameters[EndConversationSqlParameter].Value = handle;
                                    endconv.ExecuteNonQuery();
                                }
                                else
                                {
                                    var xml = XElement.Parse(message.Value);
                                    messageToInsert = message.Value;
                                    var type = xml.Descendants("Type").First().Value;
                                    var xmlData = xml.Descendants("Data").FirstOrDefault();
                                    var data = xmlData?.FirstNode?.ToString();
                                    var handler = _messageHandlers.SingleOrDefault(x => x.Type == type);

                                    if (handler == null)
                                    {
                                        Logger.Error($"Brak zdefiniowanego handlera dla typu {type}");
                                        throw new Exception($"Brak zdefiniowanego handlera dla typu {type}");
                                    }
                                    else
                                    {
                                        // Task.Run(() => handler.Handler.HandleMessage(data), _cancellationToken);
                                        try
                                        {
                                            using (var task = Task.Run(() => handler.Handler.HandleMessage(data), _cancellationToken))
                                            {

                                                task.Wait();
                                                tran.Commit();
                                                continue;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.Error($"{ex.Message}", ex);
                                            if (FrameworkConfiguration.EnableErrorCapture)
                                            {
                                                if (!string.IsNullOrEmpty(messageToInsert))
                                                {
                                                    try
                                                    {
                                                        _serviceBrokerRepository.InsertErrorMessage(messageToInsert);
                                                        messageToInsert = string.Empty;
                                                        // tran.Commit();
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        tran.Rollback();
                                                        Logger.Error(e.Message + messageToInsert, e);
                                                    }
                                                }
                                            }
                                            //else
                                            //{
                                            //    continue;
                                            //}
                                        }
                                    }
                                }
                            }
                        }
                        tran.Commit();
                        Thread.Sleep(10);

                        errorsCount = 0;
                        _exceptions = null;

                        if (!_cancellationToken.IsCancellationRequested)
                        {
                            continue;
                        }

                        _cancellationToken.ThrowIfCancellationRequested();

                        break;
                    }
                    catch (Exception e)
                    {
                        Thread.Sleep(2000);
                        errorsCount++;

                        if (_exceptions == null)
                        {
                            _exceptions = new List<Exception>();
                        }

                        _exceptions.Add(e);
                        Logger.Error(e.Message, e);

                        if (errorsCount >= ErrorsCountLimit)
                        {
                            throw new AggregateException("Wystąpiło 10 błędów powodujących zatrzymanie usługi ServiceBroker", _exceptions);
                        }

                        if (_cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }
                    }
                } while (true);
            }
            catch (Exception e)
            {
                Logger.Fatal(e.Message, e);
                throw;
            }
        }

    }
}
