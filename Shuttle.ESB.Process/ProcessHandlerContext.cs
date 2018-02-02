using Shuttle.Core.Pipelines;
using Shuttle.Core.Threading;
using Shuttle.Recall;

namespace Shuttle.Esb.Process
{
	public class ProcessHandlerContext<T> : HandlerContext<T>, IProcessHandlerContext<T> where T : class
	{
		public ProcessHandlerContext(IServiceBusConfiguration configuration, ITransportMessageFactory transportMessageFactory,
			IPipelineFactory pipelineFactory, ISubscriptionManager subscriptionManager, TransportMessage transportMessage,
			T message, IThreadState activeState,
			IKeyStore keyStore, EventStream stream) :
			base(
				configuration, transportMessageFactory, pipelineFactory, subscriptionManager, transportMessage, message, activeState
			)
		{
			KeyStore = keyStore;
			Stream = stream;
		}

		public IKeyStore KeyStore { get; }
		public EventStream Stream { get; }
	}
}