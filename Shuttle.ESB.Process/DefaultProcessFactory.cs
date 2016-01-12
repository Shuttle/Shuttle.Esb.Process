using System;

namespace Shuttle.ESB.Process
{
	public class DefaultProcessFactory : IProcessFactory
	{
		public IProcessManager Create(Type processType)
		{
			return (IProcessManager)Activator.CreateInstance(processType);
		}
	}
}