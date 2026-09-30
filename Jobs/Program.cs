using Framework.Attribiutes.Dependency;
using Framework.Configuration;
using Jobs.Enums;
using Jobs.Environment;
using Jobs.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiceBroker.System;
using System.Reflection;
using System.Text;

namespace Jobs
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            FrameworkConfiguration.SetConfiguration(builder.Build(), Assembly.GetExecutingAssembly().FullName);

            var jobType = ValidateInput(args);

            if (jobType == JobTypeEnum.Undefined)
            {
                Console.WriteLine(HelpArgs());
                return;
            }

            using IHost host = CreateHostBuilder(args).Build();

            var job = new JobFactory().Create(jobType, host.Services);
            if (job != null)
            {
                await job.Run();
            }
        }

        private static string HelpArgs()
        {
            StringBuilder sb = new();
            sb.AppendLine(string.Format("Dopuszczalne parametry wywołania"));
            sb.AppendLine();

            foreach (string job in Enum.GetNames(typeof(JobTypeEnum)))
            {
                var @enum = (JobTypeEnum)Enum.Parse(typeof(JobTypeEnum), job);
                if (@enum == JobTypeEnum.Undefined)
                    continue;

                sb.AppendLine(string.Format("JobType={0}", ((int)@enum).ToString()));
                sb.AppendLine(string.Format("\t {0} - {1}", job, @enum.GetDesrciptionOrValueString()));
            }
            return sb.ToString();
        }

        private static JobTypeEnum ValidateInput(string[] args)
        {
            if (args == null || args.Length < 1)
            {
                return JobTypeEnum.Undefined;
            }

            string[] param = args[0].Split('=');
            if (param.Length != 2)
            {
                return JobTypeEnum.Undefined;
            }

            if (param[0].ToLower() != "jobtype")
            {
                return JobTypeEnum.Undefined;
            }

            int jobTypeId;
            try
            {
                jobTypeId = int.Parse(param[1]);
            }
            catch
            {
                return JobTypeEnum.Undefined;
            }

            return jobTypeId < 1 || jobTypeId > Enum.GetValues(typeof(JobTypeEnum)).Cast<int>().Max()
                ? JobTypeEnum.Undefined
                : (JobTypeEnum)jobTypeId;
        }

       
        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            IEnumerable<Type> assemblyTypes = GetAllAssemblies(Assembly.GetExecutingAssembly())
                .SelectMany(x => x.GetTypes())
                .Where(t => t.GetCustomAttribute<DependencyInjectionAttribute>() is not null)
                .Distinct();

            return Host.CreateDefaultBuilder(args).ConfigureServices((_, services) =>
            {
                services.AddLogging(l =>
                {
                    l.AddConsole();
                    l.AddDebug();
                });
                foreach (Type type in assemblyTypes)
                {
                    DependencyInjectionAttribute? attribute = type.GetCustomAttribute<DependencyInjectionAttribute>();
                    if (attribute is null) continue;
                    switch (attribute.DependencyType)
                    {
                        case DependencyEnum.Singleton when attribute.InterfaceType is null:
                            services.AddSingleton(type);
                            break;

                        case DependencyEnum.Singleton when attribute.InterfaceType is not null:
                            services.AddSingleton(attribute.InterfaceType, type);
                            break;

                        case DependencyEnum.Transient when attribute.InterfaceType is null:
                            services.AddTransient(type);
                            break;

                        case DependencyEnum.Transient when attribute.InterfaceType is not null:
                            services.AddTransient(attribute.InterfaceType, type);
                            break;

                        case DependencyEnum.Scope when attribute.InterfaceType is not null:
                            services.AddScoped(attribute.InterfaceType, type);
                            break;

                        default:
                            services.AddScoped(type);
                            break;
                    };
                };

                services.AddMemoryCache();

                services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            });
        }

        private static IEnumerable<Assembly> GetAllAssemblies(Assembly assembly)
        {
            yield return assembly;
            foreach (Assembly subAssy in assembly.GetReferencedAssemblies().Select(x => Assembly.Load(x)))
                yield return subAssy;
        }
    }
}
