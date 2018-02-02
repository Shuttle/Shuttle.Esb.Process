using Shuttle.Core.Container;
using Shuttle.Core.Contract;

namespace Shuttle.Esb.Process
{
	public class Bootstrap :
		IComponentRegistryBootstrap,
		IComponentResolverBootstrap
	{
		private static bool _registryBootstrapCalled;
		private static bool _resolverBootstrapCalled;

		public void Register(IComponentRegistry registry)
		{
			Guard.AgainstNull(registry, "registry");

			if (_registryBootstrapCalled)
			{
				return;
			}

		    if (!registry.IsRegistered<IProcessConfiguration>())
		    {
		        registry.AttemptRegisterInstance<IProcessConfiguration>(ProcessSection.Configuration());
		    }

		    registry.AttemptRegister<IProcessActivator, DefaultProcessActivator>();
			registry.AttemptRegister<IMessageHandlerInvoker, ProcessMessageHandlerInvoker>();

			_registryBootstrapCalled = true;
		}

		public void Resolve(IComponentResolver resolver)
		{
			Guard.AgainstNull(resolver, "resolver");

			if (_resolverBootstrapCalled)
			{
				return;
			}

		    if (resolver.Resolve<IProcessActivator>() is DefaultProcessActivator processActivator)
			{
				processActivator.RegisterMappings();
			}

			_resolverBootstrapCalled = true;
		}
	}
}