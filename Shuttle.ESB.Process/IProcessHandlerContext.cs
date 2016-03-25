using Shuttle.Recall;

namespace Shuttle.Esb.Process
{
	public interface IProcessHandlerContext<out T> : IHandlerContext<T> where T : class
	{
		IKeyStore KeyStore { get; }
		EventStream Stream { get; }
	}
}