using Shuttle.Core.Container;
using Shuttle.Core.Contract;

namespace Shuttle.Esb.Process
{
	public static class ComponentRegistryExtensions
	{
		public static void RegisterProcessManagement(this IComponentRegistry registry)
		{
			Guard.AgainstNull(registry, nameof(registry));

		    if (!registry.IsRegistered<IProcessConfiguration>())
		    {
		        registry.AttemptRegisterInstance<IProcessConfiguration>(ProcessSection.Configuration());
		    }

		    registry.AttemptRegister<IProcessActivator, DefaultProcessActivator>();
			registry.AttemptRegister<IMessageHandlerInvoker, ProcessMessageHandlerInvoker>();
		}
	}
}