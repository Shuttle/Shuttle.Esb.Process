using System.Collections.Generic;

namespace Shuttle.Esb.Process
{
    public class ProcessManagementOptions
    {
        public const string SectionName = "Shuttle:ProcessManagement";

        public List<string> AssemblyNames { get; set; } = new List<string>();
    }
}