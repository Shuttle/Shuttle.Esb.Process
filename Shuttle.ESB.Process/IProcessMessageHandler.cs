namespace Shuttle.ESB.Process
{
    public interface IProcessMessageHandler<T> where T : class
    {
        void ProcessMessage(ProcessHandlerContext<T> context);
    }
}