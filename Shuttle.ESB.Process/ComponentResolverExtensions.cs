using Shuttle.Core.Container;
using Shuttle.Core.Contract;

namespace Shuttle.Esb.Process
{
	public static class ComponentResolverExtensions
	{
		public static void ResolveProcessManagement(this IComponentResolver resolver)
		{
			Guard.AgainstNull(resolver, "resolver");

		    if (resolver.Resolve<IProcessActivator>() is DefaultProcessActivator processActivator)
			{
				processActivator.RegisterMappings();
			}
		}
	}
}