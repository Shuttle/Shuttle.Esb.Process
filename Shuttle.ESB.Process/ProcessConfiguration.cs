using System;

namespace Shuttle.ESB.Process
{
    public class ProcessConfiguration : IProcessConfiguration
    {
        private static readonly object Padlock = new object();
	    private IProcessActivator _processActivator;
	    private IProcessFactory _processFactory;
	    public string ProviderName { get; set; }
        public string ConnectionString { get; set; }

	    public IProcessActivator ProcessActivator
	    {
		    get { return _processActivator ?? Synchronised(()=> _processActivator = new DefaultProcessActivator(ProcessFactory)); }
		    set { _processActivator = value; }
	    }

	    public IProcessFactory ProcessFactory
	    {
			get { return _processFactory ?? Synchronised(() => _processFactory = new DefaultProcessFactory()); }
			set { _processFactory = value; }
		}

		private static T Synchronised<T>(Func<T> f)
        {
            lock (Padlock)
            {
                return f.Invoke();
            }
        }
    }
}