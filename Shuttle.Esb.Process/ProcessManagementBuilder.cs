using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;

namespace Shuttle.Esb.Process
{
    public class ProcessManagementBuilder
    {
        private ProcessManagementOptions _processManagementOptions = new ProcessManagementOptions();

        public IServiceCollection Services { get; }

        public ProcessManagementBuilder(IServiceCollection services)
        {
            Guard.AgainstNull(services, nameof(services));

            Services = services;
        }

        public ProcessManagementOptions Options
        {
            get => _processManagementOptions;
            set => _processManagementOptions = value ?? throw new ArgumentNullException(nameof(value));
        }

        public ProcessManagementBuilder AddAssembly(Assembly assembly)
        {
            Guard.AgainstNull(assembly, nameof(assembly));

            AddAssembly(assembly.FullName);

            return this;
        }

        private ProcessManagementBuilder AddAssembly(string name)
        {
            Guard.AgainstNullOrEmptyString(name, nameof(name));

            if (!Options.AssemblyNames.Contains(name))
            {
                Options.AssemblyNames.Add(name);
            }

            return this;
        }
    }
}