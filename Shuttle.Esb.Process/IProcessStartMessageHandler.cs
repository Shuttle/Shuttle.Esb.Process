namespace Shuttle.Esb.Process
{
	public interface IProcessStartMessageHandler<in T> where T : class
	{
		void ProcessMessage(IProcessHandlerContext<T> context);
	}
}