using Shuttle.ESB.Core;
using Shuttle.Recall.Core;

namespace Shuttle.ESB.Process
{
    public interface IProcessStartMessageHandler<T> where T : class
    {
        void ProcessMessage(HandlerContext<T> context, EventStream stream);
    }
}