using System.Threading;
using Shuttle.Core.Pipelines;
using Shuttle.Core.Threading;
using Shuttle.Recall;

namespace Shuttle.Esb.Process
{
	public class ProcessHandlerContext<T> : HandlerContext<T>, IProcessHandlerContext<T> where T : class
	{
		public ProcessHandlerContext(ITransportMessageFactory transportMessageFactory,
			IPipelineFactory pipelineFactory, ISubscriptionManager subscriptionManager, TransportMessage transportMessage,
			T message, CancellationToken cancellationToken, EventStream stream) :
			base(transportMessageFactory, pipelineFactory, subscriptionManager, transportMessage, message, cancellationToken)
		{
			Stream = stream;
		}

		public EventStream Stream { get; }
	}
}