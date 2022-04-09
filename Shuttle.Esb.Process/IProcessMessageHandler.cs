namespace Shuttle.Esb.Process
{
	public interface IProcessMessageHandler<in T> where T : class
	{
		void ProcessMessage(IProcessHandlerContext<T> context);
	}
}