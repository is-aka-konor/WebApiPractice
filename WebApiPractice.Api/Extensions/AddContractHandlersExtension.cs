using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;
using WebApiPractice.Api.ValidationFlow.Interfaces;

namespace WebApiPractice.Api.Extensions
{
    public static class AddContractHandlersExtension
    {
        /// <summary>
        /// Add all classes implemented <see cref="IValidationContractHandler"/> to dependency injection 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly">Assembly to scan for classes</param>
        /// <param name="lifetime">Lifetime for dependency (default is Scoped)</param>
        public static void AddContractHandlers(
            this IServiceCollection services,
            Assembly assembly,
            ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            var contractHandlerType = typeof(IValidationContractHandler);
            var classesToInject = assembly.GetTypes()
                                            .Where(t => contractHandlerType.IsAssignableFrom(t))
                                            .ToList();
            classesToInject.ForEach(type =>
            {
                foreach (var implementedInterface in type.GetInterfaces())
                {
                    switch (lifetime)
                    {
                        case ServiceLifetime.Scoped:
                            services.AddScoped(implementedInterface, type);
                            break;
                        case ServiceLifetime.Transient:
                            services.AddTransient(implementedInterface, type);
                            break;
                        case ServiceLifetime.Singleton:
                            services.AddSingleton(implementedInterface, type);
                            break;
                    }
                }
            });
        }
    }
}
