using MasterRad.Lifetime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAssignableTo<T>(this IServiceCollection services, Assembly[] assemblies)
        {
            var lifetime = GetLifetime<T>();

            var appInterfaces = assemblies.SelectMany(a => a.DefinedTypes.Where(t => t.IsInterface && t.GetInterfaces().Contains(typeof(T))));

            foreach (var appInterface in appInterfaces)
            {
                var implementation = assemblies
                                    .SelectMany(a => a.DefinedTypes.Where(t => !t.IsInterface && t.GetInterfaces().Contains(appInterface)) )
                                    .Single();
                services.Add(new ServiceDescriptor(typeof(T), implementation, lifetime));
            }
        }

        private static ServiceLifetime GetLifetime<T>()
        {
            if (typeof(T) == typeof(IPerDependency))
                return ServiceLifetime.Transient;

            if (typeof(T) == typeof(IPerWebRequest))
                return ServiceLifetime.Scoped;

            if (typeof(T) == typeof(ISingleton))
                return ServiceLifetime.Singleton;

            throw new ApplicationException($"Invalid type {typeof(T)}");
        }
    }
}
