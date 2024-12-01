using System.Threading.Tasks;

namespace Shuttle.Esb.Process
{
	public interface IAsyncProcessStartMessageHandler<in T> where T : class
	{
		Task ProcessMessageAsync(IProcessHandlerContext<T> context);
	}
}