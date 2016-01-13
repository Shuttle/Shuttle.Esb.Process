using System;
using System.Collections.Generic;
using Shuttle.Core.Infrastructure;
using Shuttle.ESB.Core;

namespace Shuttle.ESB.Process
{
	public class DefaultProcessActivator : IProcessActivator
	{
		private static readonly List<MessageProcessType> EmptyMappings = new List<MessageProcessType>();
		private static readonly object Padlock = new object();

		private readonly Dictionary<Type, List<MessageProcessType>> _mappings =
			new Dictionary<Type, List<MessageProcessType>>();

		private readonly Dictionary<Type, Func<TransportMessage, object, MessageProcessType>> _resolvers =
			new Dictionary<Type, Func<TransportMessage, object, MessageProcessType>>();

		private readonly Func<Type, IProcessManager> _processFactoryFunction;

		public DefaultProcessActivator()
		{
			_processFactoryFunction = type => (IProcessManager)Activator.CreateInstance(type);
		}

		public DefaultProcessActivator(Func<Type, IProcessManager> processFactoryFunction)
		{
			Guard.AgainstNull(processFactoryFunction, "processFactoryFunction");

			_processFactoryFunction = processFactoryFunction;
		}

		public IProcessActivator RegisterResolver<TMessageType>(
			Func<TransportMessage, object, MessageProcessType> resolver)
		{
			Guard.AgainstNull(resolver, "resolver");

			_resolvers.Add(typeof(TMessageType), resolver);

			return this;
		}

		public IProcessActivator RegisterProcessMessage<TMessageType, TProcessType>()
		{
			RegisterProcessMessage(typeof(TMessageType), typeof(TProcessType));

			return this;
		}

		public IProcessActivator RegisterProcessMessage(Type messageType, Type processType)
		{
			RegisterProcessMessage(messageType, processType, false);

			return this;
		}

		public IProcessActivator RegisterProcessStartMessage<TMessageType, TProcessType>()
		{
			RegisterProcessStartMessage(typeof(TMessageType), typeof(TProcessType));

			return this;
		}

		public IProcessActivator RegisterProcessStartMessage(Type messageType, Type processType)
		{
			RegisterProcessMessage(messageType, processType, true);

			return this;
		}

		public bool IsProcessMessage(TransportMessage transportMessage, object message)
		{
			Guard.AgainstNull(transportMessage, "transportMessage");
			Guard.AgainstNull(message, "message");

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

			return resolver != null && resolver.Invoke(transportMessage, message) != null;
		}

		public IProcessManager Create(TransportMessage transportMessage, object message)
		{
			Guard.AgainstNull(transportMessage, "transportMessage");
			Guard.AgainstNull(message, "message");

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
						? string.Format(ProcessResources.ResolverRequiredForMultipleMappingsException,
							messageType.FullName)
						: string.Format(ProcessResources.ResolverRequiredForNoMappingException, messageType.FullName));
				}

				messageProcessType = resolver.Invoke(transportMessage, message);
			}

			IProcessManager processInstance;
			Guid correlationId;

			if (!messageProcessType.IsStartedByMessage)
			{
				if (!Guid.TryParse(transportMessage.CorrelationId, out correlationId))
				{
					throw new ProcessException(string.Format(ProcessResources.InvalidCorrelationGuid,
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
				throw new ProcessException(string.Format(ProcessResources.ProcessFactoryFunctionException, messageProcessType.ProcessType.AssemblyQualifiedName));
			}

			var result = processInstance;

			result.CorrelationId = correlationId;

			return result;
		}

		private void RegisterProcessMessage(Type messageType, Type processType, bool isStartedByMessage)
		{
			Guard.AgainstNull(messageType, "messageType");
			Guard.AgainstNull(processType, "processType");

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

		public void RegisterMappings()
		{
			RegisterMappings(typeof(IProcessMessageHandler<>), false);
			RegisterMappings(typeof(IProcessStartMessageHandler<>), true);
		}

		private void RegisterMappings(Type interfaceType, bool isStartedByMessage)
		{
			var reflectionService = new ReflectionService();

			var types = reflectionService.GetTypes(interfaceType);

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