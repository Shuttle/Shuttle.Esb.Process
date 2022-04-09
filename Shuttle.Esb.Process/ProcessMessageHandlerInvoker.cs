using System;
using Shuttle.Core.Container;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Core.Pipelines;
using Shuttle.Core.Reflection;
using Shuttle.Core.Specification;
using Shuttle.Recall;

namespace Shuttle.Esb.Process
{
    public class ProcessMessageHandlerInvoker : IMessageHandlerInvoker
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IMessageHandlerInvoker _defaultMessageHandlerInvoker;
        private readonly IEventStore _eventStore;
        private readonly IPipelineFactory _pipelineFactory;
        private readonly IProcessActivator _processActivator;
        private readonly IProcessConfiguration _processConfiguration;
        private readonly ISubscriptionManager _subscriptionManager;
        private readonly ITransportMessageFactory _transportMessageFactory;

        public ProcessMessageHandlerInvoker(IComponentResolver resolver, IProcessConfiguration processConfiguration,
            IProcessActivator processActivator, ITransportMessageFactory transportMessageFactory,
            IPipelineFactory pipelineFactory, ISubscriptionManager subscriptionManager,
            IDatabaseContextFactory databaseContextFactory, IEventStore eventStore,
            IMessageHandlingAssessor messageHandlingAssessor)
        {
            Guard.AgainstNull(resolver, nameof(resolver));
            Guard.AgainstNull(processConfiguration, nameof(processConfiguration));
            Guard.AgainstNull(processActivator, nameof(processActivator));
            Guard.AgainstNull(transportMessageFactory, nameof(transportMessageFactory));
            Guard.AgainstNull(pipelineFactory, nameof(pipelineFactory));
            Guard.AgainstNull(subscriptionManager, nameof(subscriptionManager));
            Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
            Guard.AgainstNull(eventStore, nameof(eventStore));
            Guard.AgainstNull(messageHandlingAssessor, nameof(messageHandlingAssessor));

            _transportMessageFactory = transportMessageFactory;
            _pipelineFactory = pipelineFactory;
            _subscriptionManager = subscriptionManager;
            _databaseContextFactory = databaseContextFactory;
            _eventStore = eventStore;
            _processConfiguration = processConfiguration;
            _processActivator = processActivator;
            _defaultMessageHandlerInvoker = new DefaultMessageHandlerInvoker(resolver, pipelineFactory,
                subscriptionManager, transportMessageFactory);

            foreach (var type in new ReflectionService().GetTypesAssignableTo<IProcessMessageAssessor>())
            {
                try
                {
                    var specificationInstance = Activator.CreateInstance(type);

                    messageHandlingAssessor.RegisterAssessor((ISpecification<IPipelineEvent>) specificationInstance);
                }
                catch
                {
                    throw new ProcessException(string.Format(Resources.MissingProcessAssessorConstructor,
                        type.AssemblyQualifiedName));
                }
            }
        }

        public MessageHandlerInvokeResult Invoke(IPipelineEvent pipelineEvent)
        {
            var state = pipelineEvent.Pipeline.State;
            var transportMessage = state.GetTransportMessage();
            var message = state.GetMessage();

            if (!_processActivator.IsProcessMessage(transportMessage, message))
            {
                return _defaultMessageHandlerInvoker.Invoke(pipelineEvent);
            }

            var processInstance = _processActivator.Create(transportMessage, message);

            EventStream stream;

            using (_databaseContextFactory.Create(_processConfiguration.ProviderName,
                _processConfiguration.ConnectionString))
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
                    Resources.ProcessMessageMethodMissingException,
                    processInstance.GetType().FullName,
                    messageType.FullName));
            }

            var handlerContext = Activator.CreateInstance(contextType, _transportMessageFactory, _pipelineFactory,
                _subscriptionManager, transportMessage, message, state.GetCancellationToken(), stream);

            method.Invoke(processInstance, new[] {handlerContext});

            using (_databaseContextFactory.Create(_processConfiguration.ProviderName,
                _processConfiguration.ConnectionString))
            {
                _eventStore.Save(stream);
            }

            return MessageHandlerInvokeResult.InvokedHandler(processInstance);
        }
    }
}