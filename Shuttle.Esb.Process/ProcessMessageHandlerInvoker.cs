using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;
using Shuttle.Core.Pipelines;
using Shuttle.Core.Reflection;
using Shuttle.Core.Specification;
using Shuttle.Recall;

namespace Shuttle.Esb.Process
{
    public class ProcessMessageHandlerInvoker : IMessageHandlerInvoker
    {
        private readonly IMessageHandlerInvoker _messageHandlerInvoker;
        private readonly IEventStore _eventStore;
        private readonly IMessageSender _messageSender;
        private readonly IProcessActivator _processActivator;

        public ProcessMessageHandlerInvoker(IServiceProvider serviceProvider, IOptions<ProcessManagementOptions> processManagementOptions, IMessageSender messageSender, IProcessActivator processActivator, IEventStore eventStore, IMessageHandlingSpecification messageHandlingSpecification)
        {
            Guard.AgainstNull(serviceProvider, nameof(serviceProvider));
            Guard.AgainstNull(processManagementOptions, nameof(processManagementOptions));
            Guard.AgainstNull(processManagementOptions.Value, nameof(processManagementOptions.Value));
            Guard.AgainstNull(messageHandlingSpecification, nameof(messageHandlingSpecification));
            
            _messageSender = Guard.AgainstNull(messageSender, nameof(messageSender));
            _eventStore = Guard.AgainstNull(eventStore, nameof(eventStore));
            _processActivator = Guard.AgainstNull(processActivator, nameof(processActivator));
            _messageHandlerInvoker = new MessageHandlerInvoker(serviceProvider, messageSender);

            var reflectionService = new ReflectionService();

            var assemblies = new List<Assembly>();

            assemblies.AddRange((processManagementOptions.Value.AssemblyNames ?? Enumerable.Empty<string>()).Any()
                ? processManagementOptions.Value.AssemblyNames.Select(Assembly.Load)
                : new ReflectionService().GetRuntimeAssemblies());

            foreach (var assembly in assemblies)
            {
                foreach (var type in reflectionService.GetTypesAssignableTo<IProcessMessageSpecification>(assembly))
                {
                    try
                    {
                        var specificationInstance = Activator.CreateInstance(type);

                        messageHandlingSpecification.Add((ISpecification<IPipelineEvent>)specificationInstance);
                    }
                    catch
                    {
                        throw new ProcessException(string.Format(Resources.ProcessMessageSpecificationConstructorException,
                            type.AssemblyQualifiedName));
                    }
                }
            }
        }

        public MessageHandlerInvokeResult Invoke(OnHandleMessage pipelineEvent)
        {
            return InvokeAsync(pipelineEvent, true).GetAwaiter().GetResult();
        }

        public async Task<MessageHandlerInvokeResult> InvokeAsync(OnHandleMessage pipelineEvent)
        {
            return await InvokeAsync(pipelineEvent, false);
        }

        private async Task<MessageHandlerInvokeResult> InvokeAsync(OnHandleMessage pipelineEvent, bool sync)
        {
            var state = pipelineEvent.Pipeline.State;
            var transportMessage = state.GetTransportMessage();
            var message = state.GetMessage();

            if (!_processActivator.IsProcessMessage(transportMessage, message))
            {
                return _messageHandlerInvoker.Invoke(pipelineEvent);
            }

            var processInstance = _processActivator.Create(transportMessage, message);

            var stream = sync
                ? _eventStore.Get(processInstance.CorrelationId)
                : await _eventStore.GetAsync(processInstance.CorrelationId).ConfigureAwait(false);

            stream.Apply(processInstance);

            var messageType = message.GetType();
            var contextType = typeof(ProcessHandlerContext<>).MakeGenericType(messageType);
            var processType = processInstance.GetType();
            var method = sync 
                ? processType.GetMethod("ProcessMessage", new[] {contextType})
                : processType.GetMethod("ProcessMessageAsync", new[] {contextType});

            if (method == null)
            {
                throw new ProcessMessageMethodMissingException(string.Format(
                    sync ? Resources.ProcessMessageMethodMissingException : Resources.ProcessMessageAsyncMethodMissingException,
                    processInstance.GetType().FullName,
                    messageType.FullName));
            }

            var handlerContext = Activator.CreateInstance(contextType, stream, _messageSender, transportMessage, message, state.GetCancellationToken());

            method.Invoke(processInstance, new[] {handlerContext});

            if (sync)
            {
                _eventStore.Save(stream);
            }
            else
            {
                await _eventStore.SaveAsync(stream).ConfigureAwait(false);
            }

            return MessageHandlerInvokeResult.InvokedHandler(processInstance.GetType().AssemblyQualifiedName);
        }
    }
}