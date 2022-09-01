using System;
using System.Reflection;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Esb.Process
{
    public class ProcessManagementOptionsValidator : IValidateOptions<ProcessManagementOptions>
    {
        public ValidateOptionsResult Validate(string name, ProcessManagementOptions options)
        {
            Guard.AgainstNull(options, nameof(options));

            if (string.IsNullOrWhiteSpace(options.ConnectionStringName))
            {
                return ValidateOptionsResult.Fail(Resources.ConnectionStringNameException);
            }

            foreach (var assemblyName in options.AssemblyNames)
            {
                try
                {
                    Assembly.Load(assemblyName);
                }
                catch
                {
                    return ValidateOptionsResult.Fail(string.Format(Resources.AssemblyNameException, assemblyName));
                }
            }

            return ValidateOptionsResult.Success;
        }
    }
}