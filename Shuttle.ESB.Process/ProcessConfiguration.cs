using System;

namespace Shuttle.Esb.Process
{
	public class ProcessConfiguration : IProcessConfiguration
	{
		private static readonly object Padlock = new object();
		private IProcessActivator _processActivator;
		public string ProviderName { get; set; }
		public string ConnectionString { get; set; }

		public IProcessActivator ProcessActivator
		{
			get { return _processActivator ?? Synchronised(() => _processActivator = new DefaultProcessActivator()); }
			set { _processActivator = value; }
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