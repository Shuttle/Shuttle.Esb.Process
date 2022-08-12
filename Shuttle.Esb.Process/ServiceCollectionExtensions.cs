using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shuttle.Core.Contract;

namespace Shuttle.Esb.Process
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProcessManagement(this IServiceCollection services, Action<ProcessManagementBuilder> builder = null)
        {
            Guard.AgainstNull(services, nameof(services));

            var processManagementBuilder = new ProcessManagementBuilder(services);

            builder?.Invoke(processManagementBuilder);

            services.TryAddSingleton<IProcessActivator, ProcessActivator>();
            services.TryAddSingleton<IMessageHandlerInvoker, ProcessMessageHandlerInvoker>();

            services.AddOptions<ProcessManagementOptions>().Configure(options =>
            {
                options.ConnectionStringName = processManagementBuilder.Options.ConnectionStringName;
                options.AssemblyNames = processManagementBuilder.Options.AssemblyNames;
            });

            return services;
        }
    }
}