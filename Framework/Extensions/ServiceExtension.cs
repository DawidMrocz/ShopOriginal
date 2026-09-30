using Framework.Attribiutes.Dependency;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Framework.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddDependencyInjections(this IServiceCollection services, Assembly assembly)
        {
            IEnumerable<Type> assemblyTypes = GetAllAssemblies(assembly)
                .SelectMany(x => x.GetTypes())
                .Where(t => t.GetCustomAttribute<DependencyInjectionAttribute>() is not null)
                .Distinct();

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

            return services;
        }
        private static IEnumerable<Assembly> GetAllAssemblies(Assembly assembly)
        {
            yield return assembly;
            foreach (Assembly subAssy in assembly.GetReferencedAssemblies().Select(x => Assembly.Load(x)))
                yield return subAssy;
        }
    }
}
