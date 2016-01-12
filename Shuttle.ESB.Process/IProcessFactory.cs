using System;

namespace Shuttle.ESB.Process
{
	public interface IProcessFactory
	{
		IProcessManager Create(Type processType);
	}
}