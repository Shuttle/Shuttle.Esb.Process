using System.Collections.Generic;

namespace Shuttle.Esb.Process
{
    public class ProcessManagementOptions
    {
        public const string SectionName = "Shuttle:ProcessManagement";

        public string ConnectionStringName { get; set; }

        public List<string> AssemblyNames { get; set; } = new List<string>();
    }
}