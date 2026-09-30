using Framework.Attribiutes.Dependency;
using log4net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using ServiceBroker.Attributes;
using ServiceBroker.Handler;
using ServiceBroker.Interfaces;
using ServiceBroker.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBroker.System
{
    public static class Dependencies
    {
        public static IEnumerable<Models.MessageHandler> LoadHandlers()
        {
            var messageHandlers = new List<Models.MessageHandler>();
            var assemblies = GetAssembliesForDomain().SelectMany(s => s.GetTypes()).Where(p =>
                p.IsClass && !p.IsAbstract && typeof(IMessageHandler).IsAssignableFrom(p));


            IServiceProvider provider = ProviderHelper.GetServiceProvider();

            foreach (var assembly in assemblies)
            {
                var instance = (IMessageHandler)Activator.CreateInstance(assembly, provider);
                var attribute = assembly.GetCustomAttribute<MessageTypeAttribute>();

                if (attribute == null)
                {
                    throw new NotSupportedException("Brak zadeklarowanego typu dla klasy implementującej");
                }

                messageHandlers.Add(new Models.MessageHandler
                {
                    Type = attribute.Type,
                    Handler = instance
                });
            }
            return messageHandlers;
        }

        private static IEnumerable<Assembly> GetAssembliesForDomain()
        {
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "MessageHandlers"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "MessageHandlers");
            }

            var files = Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory + "MessageHandlers", "*.dll");
            return files.Select(Assembly.LoadFrom);
        }
    }

    public interface IDependency
    {
    }

    public interface ISingletonDependency : IDependency
    {
    }

    public interface ITransientDependency : IDependency
    {
    }

    

    public static class ProviderHelper
    {
        private static IEnumerable<Assembly> GetAllAssemblies(Assembly assembly)
        {
            yield return assembly;
            foreach (Assembly subAssy in assembly.GetReferencedAssemblies().Select(x => Assembly.Load(x)))
                yield return subAssy;
        }
        public static IServiceProvider GetServiceProvider()
        {
            IEnumerable<Type> assemblyTypes = GetAllAssemblies(Assembly.GetExecutingAssembly())
                .SelectMany(x => x.GetTypes())
                .Where(t => t.GetCustomAttribute<DependencyInjectionAttribute>() is not null)
                .Distinct();

            var services = new ServiceCollection();

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

            return services.BuildServiceProvider();
        }
    }

    public static class AssemblyHelper
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(AssemblyHelper));

        private static IEnumerable<Assembly> _assemblies = null;

        public static IEnumerable<Assembly> Assemblies
        {
            get
            {
                if (_assemblies == null)
                {
                    _assemblies = GetAssembliesForDomain();
                }

                return _assemblies;
            }
        }

        public static IEnumerable<Type> GetImplementationType(Func<Type, bool> predicate)
        {
            foreach (Type item in Assemblies.SelectMany((Assembly assembly) => assembly.GetTypes()).Where(predicate))
            {
                yield return item;
            }
        }

        private static IEnumerable<Assembly> GetAssembliesForDomain()
        {
            List<Assembly> list = new List<Assembly>();
            List<string> list2 = new List<string>();
            List<string> source = (from f in Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll", SearchOption.AllDirectories)
                                   where !f.EndsWith("\\sni.dll")
                                   select f).ToList();
            foreach (string item in source.OrderBy((string x) => Path.GetDirectoryName(x).Length))
            {
                if (!list2.Select((string x) => Path.GetFileName(x)).Contains(Path.GetFileName(item)))
                {
                    Console.WriteLine(item);
                    list2.Add(item);
                }
            }

            foreach (string item2 in list2)
            {
                try
                {
                    list.Add(AssemblyLoadContext.Default.LoadFromAssemblyPath(item2));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd przy ładowaniu Assembly {item2}. Message : {ex.Message} ");
                }
            }

            return list;
        }
    }
}
