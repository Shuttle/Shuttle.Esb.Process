using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;
using Shuttle.Core.Reflection;

namespace Shuttle.Esb.Process
{
	public class ProcessActivator : IProcessActivator
	{
		private static readonly List<MessageProcessType> EmptyMappings = new List<MessageProcessType>();
		private static readonly object Padlock = new object();

		private readonly Dictionary<Type, List<MessageProcessType>> _mappings =
			new Dictionary<Type, List<MessageProcessType>>();

		private readonly Dictionary<Type, Func<TransportMessage, object, MessageProcessType>> _resolvers =
			new Dictionary<Type, Func<TransportMessage, object, MessageProcessType>>();

		private readonly Func<Type, IProcessManager> _processFactoryFunction;
		private readonly ReflectionService _reflectionService = new ReflectionService();

		public ProcessActivator(IOptions<ProcessManagementOptions> processManagementOptions)
		{
			Guard.AgainstNull(processManagementOptions, nameof(processManagementOptions));
			Guard.AgainstNull(processManagementOptions.Value, nameof(processManagementOptions.Value));

			_processFactoryFunction = type => (IProcessManager) Activator.CreateInstance(type);

			foreach (var assemblyName in processManagementOptions.Value.AssemblyNames)
			{
				var assembly = Assembly.Load(assemblyName);

				RegisterMappings(assembly, typeof(IProcessMessageHandler<>), false);
				RegisterMappings(assembly, typeof(IProcessStartMessageHandler<>), true);
			}
		}

		public IProcessActivator RegisterResolver<TMessageType>(
			Func<TransportMessage, object, MessageProcessType> resolver)
		{
			Guard.AgainstNull(resolver, nameof(resolver));

			_resolvers.Add(typeof (TMessageType), resolver);

			return this;
		}

		public IProcessActivator RegisterProcessMessage<TMessageType, TProcessType>()
		{
			RegisterProcessMessage(typeof (TMessageType), typeof (TProcessType));

			return this;
		}

		public IProcessActivator RegisterProcessMessage(Type messageType, Type processType)
		{
			RegisterProcessMessage(messageType, processType, false);

			return this;
		}

		public IProcessActivator RegisterProcessStartMessage<TMessageType, TProcessType>()
		{
			RegisterProcessStartMessage(typeof (TMessageType), typeof (TProcessType));

			return this;
		}

		public IProcessActivator RegisterProcessStartMessage(Type messageType, Type processType)
		{
			RegisterProcessMessage(messageType, processType, true);

			return this;
		}

		public bool IsProcessMessage(TransportMessage transportMessage, object message)
		{
			Guard.AgainstNull(transportMessage, nameof(transportMessage));
			Guard.AgainstNull(message, nameof(message));

			var messageType = message.GetType();

			List<MessageProcessType> mappings;

			lock (Padlock)
			{
				mappings = !_mappings.ContainsKey(messageType)
					? EmptyMappings
					: _mappings[messageType];
			}

			if (mappings.Count > 0)
			{
				return true;
			}

			var resolver = FindResolver(messageType);

			return resolver?.Invoke(transportMessage, message) != null;
		}

		public IProcessManager Create(TransportMessage transportMessage, object message)
		{
			Guard.AgainstNull(transportMessage, nameof(transportMessage));
			Guard.AgainstNull(message, nameof(message));

			var messageType = message.GetType();
			List<MessageProcessType> mappings;

			lock (Padlock)
			{
				mappings = !_mappings.ContainsKey(messageType)
					? EmptyMappings
					: _mappings[messageType];
			}

			MessageProcessType messageProcessType;

			if (mappings.Count == 1)
			{
				messageProcessType = mappings[0];
			}
			else
			{
				var resolver = FindResolver(messageType);

				if (resolver == null)
				{
					throw new ProcessException(mappings.Count > 1
						? string.Format(Resources.ResolverRequiredForMultipleMappingsException,
							messageType.FullName)
						: string.Format(Resources.ResolverRequiredForNoMappingException, messageType.FullName));
				}

				messageProcessType = resolver.Invoke(transportMessage, message);
			}

			IProcessManager processInstance;
			Guid correlationId;

			if (!messageProcessType.IsStartedByMessage)
			{
				if (!Guid.TryParse(transportMessage.CorrelationId, out correlationId))
				{
					throw new ProcessException(string.Format(Resources.InvalidCorrelationGuid,
						messageProcessType.ProcessType.FullName, messageType.FullName,
						transportMessage.CorrelationId));
				}
			}
			else
			{
				correlationId = Guid.NewGuid();
			}

			try
			{
				processInstance = _processFactoryFunction.Invoke(messageProcessType.ProcessType);
			}
			catch
			{
				throw new ProcessException(string.Format(Resources.ProcessFactoryFunctionException,
					messageProcessType.ProcessType.AssemblyQualifiedName));
			}

			var result = processInstance;

			result.CorrelationId = correlationId;

			return result;
		}

		private void RegisterProcessMessage(Type messageType, Type processType, bool isStartedByMessage)
		{
			Guard.AgainstNull(messageType, nameof(messageType));
			Guard.AgainstNull(processType, nameof(processType));

			lock (Padlock)
			{
				if (!_mappings.ContainsKey(messageType))
				{
					_mappings.Add(messageType, new List<MessageProcessType>());
				}

				_mappings[messageType].Add(new MessageProcessType(processType, isStartedByMessage));
			}
		}

		private Func<TransportMessage, object, MessageProcessType> FindResolver(Type messageType)
		{
			return !_resolvers.ContainsKey(messageType) ? null : _resolvers[messageType];
		}

		private void RegisterMappings(Assembly assembly, Type interfaceType, bool isStartedByMessage)
		{
			var types = _reflectionService.GetTypesAssignableTo(interfaceType, assembly);

			foreach (var type in types)
			{
				foreach (var implementedInterface in type.GetInterfaces())
				{
					if (!implementedInterface.IsGenericType)
					{
						continue;
					}

					var genericArgument = implementedInterface.GetGenericArguments()[0];

					if (implementedInterface != interfaceType.MakeGenericType(genericArgument))
					{
						continue;
					}

					RegisterProcessMessage(genericArgument, type, isStartedByMessage);
				}
			}
		}
	}
}