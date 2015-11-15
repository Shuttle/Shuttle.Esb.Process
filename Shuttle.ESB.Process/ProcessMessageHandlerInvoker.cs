using System;
using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;
using Shuttle.ESB.Core;
using Shuttle.Recall.Core;

namespace Shuttle.ESB.Process
{
    public class ProcessMessageHandlerInvoker : IMessageHandlerInvoker
    {
        private readonly IProcessConfiguration _configuration;
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IMessageHandlerInvoker _defaultMessageHandlerInvoker;
        private readonly IEventStore _eventStore;
        private readonly IKeyStore _keyStore;

        public ProcessMessageHandlerInvoker(IDatabaseContextFactory databaseContextFactory, IEventStore eventStore, IKeyStore keyStore,
            IProcessConfiguration configuration)
        {
            Guard.AgainstNull(databaseContextFactory, "databaseContextFactory");
            Guard.AgainstNull(eventStore, "eventStore");
            Guard.AgainstNull(eventStore, "keyStore");
            Guard.AgainstNull(configuration, "configuration");

            _databaseContextFactory = databaseContextFactory;
            _eventStore = eventStore;
            _keyStore = keyStore;
            _configuration = configuration;

            _defaultMessageHandlerInvoker = new DefaultMessageHandlerInvoker();
        }

        public MessageHandlerInvokeResult Invoke(PipelineEvent pipelineEvent)
        {
            var state = pipelineEvent.Pipeline.State;
            var transportMessage = state.GetTransportMessage();
            var message = state.GetMessage();

            if ( !_configuration.ProcessActivator.IsProcessMessage(transportMessage, message))
            {
                return _defaultMessageHandlerInvoker.Invoke(pipelineEvent);
            }

            var processInstance = _configuration.ProcessActivator.Create(transportMessage, message);

            EventStream stream;

            using (_databaseContextFactory.Create(_configuration.ProviderName, _configuration.ConnectionString))
            {
                stream = _eventStore.Get(processInstance.CorrelationId);
            }

            stream.Apply(processInstance);

            var messageType = message.GetType();
            var contextType = typeof(ProcessHandlerContext<>).MakeGenericType(messageType);
            var processType = processInstance.GetType();
            var method = processType.GetMethod("ProcessMessage", new[] { contextType });

            if (method == null)
            {
                throw new ProcessMessageMethodMissingException(string.Format(
                    ESBResources.ProcessMessageMethodMissingException,
                    processInstance.GetType().FullName,
                    messageType.FullName));
            }

            var handlerContext = Activator.CreateInstance(contextType, state.GetServiceBus(), transportMessage, message, state.GetActiveState(), _keyStore, stream);

            method.Invoke(processInstance, new[] { handlerContext, stream });

            using (_databaseContextFactory.Create(_configuration.ProviderName, _configuration.ConnectionString))
            {
                _eventStore.SaveEventStream(stream);
            }

            return MessageHandlerInvokeResult.InvokedHandler(processInstance);
        }
    }
}