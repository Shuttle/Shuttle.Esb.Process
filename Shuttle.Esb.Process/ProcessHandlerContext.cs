using System.Threading;
using Shuttle.Core.Contract;
using Shuttle.Recall;

namespace Shuttle.Esb.Process
{
	public class ProcessHandlerContext<T> : HandlerContext<T>, IProcessHandlerContext<T> where T : class
	{
		public ProcessHandlerContext(EventStream stream, IMessageSender messageSender, TransportMessage transportMessage,
			T message, CancellationToken cancellationToken) :
			base(messageSender, transportMessage, message, cancellationToken)
		{
			Guard.AgainstNull(stream, nameof(stream));
			Stream = stream;
		}

		public EventStream Stream { get; }
	}
}