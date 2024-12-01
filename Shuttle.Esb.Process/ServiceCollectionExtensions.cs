using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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

            services.AddSingleton<IProcessActivator, ProcessActivator>();
            services.AddSingleton<IMessageHandlerInvoker, ProcessMessageHandlerInvoker>();
            services.AddSingleton<IValidateOptions<ProcessManagementOptions>, ProcessManagementOptionsValidator>();

            services.AddOptions<ProcessManagementOptions>().Configure(options =>
            {
                options.AssemblyNames = processManagementBuilder.Options.AssemblyNames;
            });

            return services;
        }
    }
}