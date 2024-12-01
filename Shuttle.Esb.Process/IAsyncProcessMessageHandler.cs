using System.Threading.Tasks;

namespace Shuttle.Esb.Process
{
	public interface IAsyncProcessMessageHandler<in T> where T : class
	{
		Task ProcessMessageAsync(IProcessHandlerContext<T> context);
	}
}