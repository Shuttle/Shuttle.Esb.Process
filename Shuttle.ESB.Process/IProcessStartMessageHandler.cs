namespace Shuttle.ESB.Process
{
    public interface IProcessStartMessageHandler<T> where T : class
    {
        void ProcessMessage(ProcessHandlerContext<T> context);
    }
}