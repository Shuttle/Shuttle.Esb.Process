namespace Shuttle.ESB.Process
{
    public interface IProcessStartMessageHandler<T> where T : class
    {
        void ProcessMessage(IProcessHandlerContext<T> context);
    }
}