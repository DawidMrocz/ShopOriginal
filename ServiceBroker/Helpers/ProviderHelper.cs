using Microsoft.Extensions.DependencyInjection;
using ServiceBroker.System;

namespace ServiceBroker.Helpers
{
    public static class ProviderHelper
    {
        public static IServiceProvider GetServiceProvider()
        {
            var assemblyList = AssemblyHelper.Assemblies;
            var typeList = assemblyList
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t =>
                    typeof(IDependency).IsAssignableFrom(t) &&
                    !t.IsAbstract &&
                    !t.IsGenericTypeDefinition &&
                    !t.IsInterface
                );
            var services = new ServiceCollection();

            foreach (var type in typeList)
            {
                var interfaces = type.GetInterfaces();
                //jeśli typ ma klasę bazową, to ignorujemy jej interfejsy
                if (type.BaseType != null)
                {
                    var baseTypeInterfaces = type.BaseType.GetInterfaces();
                    interfaces = interfaces.Where(i => !baseTypeInterfaces.Any(bi => bi.FullName == i.FullName)).ToArray();
                }
                var typeInterface = interfaces.FirstOrDefault(i =>
                    i != typeof(IDependency) &&
                    i != typeof(ISingletonDependency) &&
                    i != typeof(ITransientDependency));
                if (typeInterface == null) continue;

                //care of type checking, IDependency must be last as is a root of all auto-implemented interfaces
                if (typeof(ITransientDependency).IsAssignableFrom(type))
                    services.AddTransient(typeInterface, type);
                else if (typeof(ISingletonDependency).IsAssignableFrom(type))
                    services.AddSingleton(typeInterface, type);
                else //IDependency
                    services.AddScoped(typeInterface, type);
            }

            return services.BuildServiceProvider();
        }
    }
}
