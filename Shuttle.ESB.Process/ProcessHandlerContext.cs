using Shuttle.Core.Infrastructure;
using Shuttle.ESB.Core;
using Shuttle.Recall.Core;

namespace Shuttle.Esb.Process
{
    public class ProcessHandlerContext<T> : HandlerContext<T>, IProcessHandlerContext<T> where T : class
    {
        public IKeyStore KeyStore { get; private set; }
        public EventStream Stream { get; private set; }

        public ProcessHandlerContext(IServiceBus bus, TransportMessage transportMessage, T message, IThreadState activeState, IKeyStore keyStore, EventStream stream) : 
            base(bus, transportMessage, message, activeState)
        {
            KeyStore = keyStore;
            Stream = stream;
        }
    }
}