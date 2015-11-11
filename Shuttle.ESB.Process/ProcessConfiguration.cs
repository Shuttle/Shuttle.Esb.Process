using System;

namespace Shuttle.ESB.Process
{
    public class ProcessConfiguration : IProcessConfiguration
    {
        private static object Padlock = new object();
        public string ProviderName { get; set; }
        public string ConnectionString { get; set; }
        
        public IProcessActivator ProcessActivator { get; set; }

        private static T Synchronised<T>(Func<T> f)
        {
            lock (Padlock)
            {
                return f.Invoke();
            }
        }
    }
}