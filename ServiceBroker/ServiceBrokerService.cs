using Microsoft.Extensions.Hosting;
using ServiceBroker.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBroker
{
    public partial class ServiceBrokerService : IHostedService
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly Task _serviceBrokerTask;

        public ServiceBrokerService()
        {
            _serviceBrokerTask = new Task(() =>
            {
                try
                {
                    var serviceBrokerHandlerService = new ServiceBrokerHandlerService(_cancellationTokenSource.Token);
                    serviceBrokerHandlerService.StartListening();
                }
                catch (AggregateException ex)
                {
                    foreach (var e in ex.InnerExceptions)
                        //Logger.Error(e.Message, e);
                    Environment.Exit(-100);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                   // Logger.Error(ex.Message, ex);
                    Environment.Exit(-50);
                }
            });
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _serviceBrokerTask.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //Logger.Error(ex.Message, ex);
                throw;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {

               // Logger.Info("OnStop");
                _cancellationTokenSource.Cancel();
                _serviceBrokerTask.Wait(10000);
                _cancellationTokenSource.Dispose();
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                   // Logger.Error(e.Message, e);
                Environment.Exit(-100);
            }
            catch (Exception ex)
            {
               // Logger.Error(ex.Message, ex);
                Environment.Exit(-50);
            }
        }
    }
}
