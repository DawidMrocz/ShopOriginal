using Framework.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace ServiceBroker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            FrameworkConfiguration.SetConfiguration(builder.Build(), "ServiceBroker");

            if (args.Length > 0)
            {
                if (args[0] == "h")
                    DisplayHelp();
                else if (args[0] == "d")
                {
                    Thread.Sleep(10000);
                    CreateHostBuilder(args).Build().Run();
                }
                else
                {
                    Console.WriteLine("Nie rozpoznany parametr na pozycji 0");
                    DisplayHelp();
                }
            }
            else
                CreateHostBuilder(args).Build().Run();
            return;
        }
        static void DisplayHelp()
        {
            Console.WriteLine("Mroczware.ServiceBroker available parameters");

            Console.WriteLine("i");
            Console.WriteLine("Instalacja usługi");

            Console.WriteLine("u");
            Console.WriteLine("Odinstalowanie usługi");

            Console.WriteLine("h, ?");
            Console.WriteLine("Wyświetla ekran dostępnych parametrów");

            Console.WriteLine("d");
            Console.WriteLine("Uruchomienie programu w wersji developerskiej (start opóźniony o 10 sekund)");

            Console.ReadLine();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddDistributedMemoryCache();
                    services.AddMemoryCache();
                    services.AddHostedService<ServiceBrokerService>();
                });
    }
}
