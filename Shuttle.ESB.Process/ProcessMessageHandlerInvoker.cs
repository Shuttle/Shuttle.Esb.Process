using System;
using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;
using Shuttle.Recall;

namespace Shuttle.Esb.Process
{
	public class ProcessMessageHandlerInvoker : IMessageHandlerInvoker
	{
		private readonly IDatabaseContextFactory _databaseContextFactory;
		private readonly IMessageHandlerInvoker _defaultMessageHandlerInvoker;
		private readonly IEventStore _eventStore;
		private readonly IKeyStore _keyStore;
		private readonly IMessageHandlingAssessor _messageHandlingAssessor;
		private readonly IPipelineFactory _pipelineFactory;
		private readonly IProcessConfiguration _processConfiguration;
		private readonly IServiceBusConfiguration _serviceBusConfiguration;
		private readonly ISubscriptionManager _subscriptionManager;
		private readonly ITransportMessageFactory _transportMessageFactory;

		public ProcessMessageHandlerInvoker(IServiceBusConfiguration serviceBusConfiguration,
			ITransportMessageFactory transportMessageFactory,
			IPipelineFactory pipelineFactory, ISubscriptionManager subscriptionManager,
			IProcessConfiguration processConfiguration, IDatabaseContextFactory databaseContextFactory, IEventStore eventStore,
			IKeyStore keyStore, IMessageHandlingAssessor messageHandlingAssessor)
		{
			Guard.AgainstNull(serviceBusConfiguration, "serviceBusConfiguration");
			Guard.AgainstNull(transportMessageFactory, "transportMessageFactory");
			Guard.AgainstNull(pipelineFactory, "pipelineFactory");
			Guard.AgainstNull(subscriptionManager, "subscriptionManager");
			Guard.AgainstNull(databaseContextFactory, "databaseContextFactory");
			Guard.AgainstNull(eventStore, "eventStore");
			Guard.AgainstNull(eventStore, "keyStore");
			Guard.AgainstNull(processConfiguration, "processConfiguration");
			Guard.AgainstNull(messageHandlingAssessor, "messageHandlingAssessor");

			_serviceBusConfiguration = serviceBusConfiguration;
			_transportMessageFactory = transportMessageFactory;
			_pipelineFactory = pipelineFactory;
			_subscriptionManager = subscriptionManager;
			_databaseContextFactory = databaseContextFactory;
			_eventStore = eventStore;
			_keyStore = keyStore;
			_messageHandlingAssessor = messageHandlingAssessor;
			_processConfiguration = processConfiguration;

			_defaultMessageHandlerInvoker = new DefaultMessageHandlerInvoker(serviceBusConfiguration);

			foreach (var type in new ReflectionService().GetTypes<IProcessMessageAssessor>())
			{
				try
				{
					var specificationInstance = Activator.CreateInstance(type);

					_messageHandlingAssessor.RegisterAssessor((ISpecification<IPipelineEvent>) specificationInstance);
				}
				catch
				{
					throw new ProcessException(string.Format(ProcessResources.MissingProcessAssessorConstructor,
						type.AssemblyQualifiedName));
				}
			}
		}

		public MessageHandlerInvokeResult Invoke(IPipelineEvent pipelineEvent)
		{
			var state = pipelineEvent.Pipeline.State;
			var transportMessage = state.GetTransportMessage();
			var message = state.GetMessage();

			if (!_processConfiguration.ProcessActivator.IsProcessMessage(transportMessage, message))
			{
				return _defaultMessageHandlerInvoker.Invoke(pipelineEvent);
			}

			var processInstance = _processConfiguration.ProcessActivator.Create(transportMessage, message);

			EventStream stream;

			using (_databaseContextFactory.Create(_processConfiguration.ProviderName, _processConfiguration.ConnectionString))
			{
				stream = _eventStore.Get(processInstance.CorrelationId);
			}

			stream.Apply(processInstance);

			var messageType = message.GetType();
			var contextType = typeof(ProcessHandlerContext<>).MakeGenericType(messageType);
			var processType = processInstance.GetType();
			var method = processType.GetMethod("ProcessMessage", new[] {contextType});

			if (method == null)
			{
				throw new ProcessMessageMethodMissingException(string.Format(
					EsbResources.ProcessMessageMethodMissingException,
					processInstance.GetType().FullName,
					messageType.FullName));
			}

			var handlerContext = Activator.CreateInstance(contextType, _serviceBusConfiguration, _pipelineFactory,
				_subscriptionManager, transportMessage, message,
				state.GetActiveState(), _keyStore, stream);

			method.Invoke(processInstance, new[] {handlerContext});

			using (_databaseContextFactory.Create(_processConfiguration.ProviderName, _processConfiguration.ConnectionString))
			{
				_eventStore.Save(stream);
			}

			return MessageHandlerInvokeResult.InvokedHandler(processInstance);
		}
	}
}