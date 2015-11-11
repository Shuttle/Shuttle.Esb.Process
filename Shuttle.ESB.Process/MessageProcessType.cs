using System;

namespace Shuttle.ESB.Process
{
    public class MessageProcessType
    {
        public MessageProcessType(Type processType, bool isStartedByMessage)
        {
            ProcessType = processType;
            IsStartedByMessage = isStartedByMessage;
        }

        public Type ProcessType { get; private set; }
        public bool IsStartedByMessage { get; private set; }
    }
}