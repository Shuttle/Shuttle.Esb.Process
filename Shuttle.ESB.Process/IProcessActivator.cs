using System;
using Shuttle.ESB.Core;

namespace Shuttle.ESB.Process
{
    public interface IProcessActivator
    {
        IProcessActivator RegisterResolver<TMessageType>(Func<TransportMessage, object, MessageProcessType> resolver);
        IProcessActivator RegisterProcessMessage<TMessageType, TProcessType>();
        IProcessActivator RegisterProcessMessage(Type messageType, Type processType);
        IProcessActivator RegisterProcessStartMessage<TMessageType, TProcessType>();
        IProcessActivator RegisterProcessStartMessage(Type messageType, Type processType);
        bool IsProcessMessage(TransportMessage transportMessage, object message);
        IProcessManager Create(TransportMessage transportMessage, object message);
    }
}