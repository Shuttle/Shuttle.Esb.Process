using Shuttle.Core.Infrastructure;

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

			registry.AttemptRegister<IProcessConfiguration>(ProcessSection.Configuration());
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

			var processActivator = resolver.Resolve<IProcessActivator>() as DefaultProcessActivator;

			if (processActivator != null)
			{
				processActivator.RegisterMappings();
			}

			_resolverBootstrapCalled = true;
		}
	}
}