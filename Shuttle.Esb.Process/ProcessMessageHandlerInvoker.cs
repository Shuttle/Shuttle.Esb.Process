using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Options;
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
        private readonly IMessageHandlerInvoker _messageHandlerInvoker;
        private readonly IEventStore _eventStore;
        private readonly IMessageSender _messageSender;
        private readonly IProcessActivator _processActivator;
        private readonly string _providerName;
        private readonly string _connectionString;

        public ProcessMessageHandlerInvoker(IServiceProvider serviceProvider, IOptionsMonitor<ConnectionStringOptions> connectionStringOptions, IOptions<ProcessManagementOptions> processManagementOptions, IMessageSender messageSender, IProcessActivator processActivator, IDatabaseContextFactory databaseContextFactory, IEventStore eventStore, IMessageHandlingAssessor messageHandlingAssessor)
        {
            Guard.AgainstNull(serviceProvider, nameof(serviceProvider));
            Guard.AgainstNull(connectionStringOptions, nameof(connectionStringOptions));
            Guard.AgainstNull(processManagementOptions, nameof(processManagementOptions));
            Guard.AgainstNull(processManagementOptions.Value, nameof(processManagementOptions.Value));
            Guard.AgainstNull(messageSender, nameof(messageSender));
            Guard.AgainstNull(processActivator, nameof(processActivator));
            Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
            Guard.AgainstNull(eventStore, nameof(eventStore));
            Guard.AgainstNull(messageHandlingAssessor, nameof(messageHandlingAssessor));

            _messageSender = messageSender;
            _databaseContextFactory = databaseContextFactory;
            _eventStore = eventStore;
            _processActivator = processActivator;
            _messageHandlerInvoker = new MessageHandlerInvoker(serviceProvider, messageSender);

            var options = connectionStringOptions.Get(processManagementOptions.Value.ConnectionStringName);

            if (options == null)
            {
                throw new InvalidOperationException(string.Format(
                    Core.Data.Resources.ConnectionStringMissingException,
                    processManagementOptions.Value.ConnectionStringName));
            }

            _providerName = options.ProviderName;
            _connectionString = options.ConnectionString;

            var reflectionService = new ReflectionService();

            var assemblies = new List<Assembly>();

            assemblies.AddRange((processManagementOptions.Value.AssemblyNames ?? Enumerable.Empty<string>()).Any()
                ? processManagementOptions.Value.AssemblyNames.Select(Assembly.Load)
                : new ReflectionService().GetRuntimeAssemblies());

            foreach (var assembly in assemblies)
            {
                foreach (var type in reflectionService.GetTypesAssignableTo<IProcessMessageAssessor>(assembly))
                {
                    try
                    {
                        var specificationInstance = Activator.CreateInstance(type);

                        messageHandlingAssessor.RegisterAssessor((ISpecification<IPipelineEvent>)specificationInstance);
                    }
                    catch
                    {
                        throw new ProcessException(string.Format(Resources.MissingProcessAssessorConstructor,
                            type.AssemblyQualifiedName));
                    }
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
                return _messageHandlerInvoker.Invoke(pipelineEvent);
            }

            var processInstance = _processActivator.Create(transportMessage, message);

            EventStream stream;

            using (_databaseContextFactory.Create(_providerName, _connectionString))
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

            var handlerContext = Activator.CreateInstance(contextType, stream, _messageSender, transportMessage, message, state.GetCancellationToken());

            method.Invoke(processInstance, new[] {handlerContext});

            using (_databaseContextFactory.Create(_providerName, _connectionString))
            {
                _eventStore.Save(stream);
            }

            return MessageHandlerInvokeResult.InvokedHandler(processInstance);
        }
    }
}