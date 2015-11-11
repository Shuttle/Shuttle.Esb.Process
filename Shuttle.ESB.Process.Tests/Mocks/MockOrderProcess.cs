using System;
using Shuttle.ESB.Core;
using Shuttle.Recall.Core;

namespace Shuttle.ESB.Process.Tests
{
    public class MockOrderProcess :
        IProcessManager,
        IProcessStartMessageHandler<MockRegisterOrderCommand>,
        IProcessMessageHandler<MockEMailSentEvent>,
        IProcessMessageHandler<MockCompleteOrderCommand>
    {
        public MockOrderProcess(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; private set; }

        public void ProcessMessage(HandlerContext<MockCompleteOrderCommand> context, EventStream stream)
        {
            throw new NotImplementedException();
        }

        public void ProcessMessage(HandlerContext<MockEMailSentEvent> context, EventStream stream)
        {
            throw new NotImplementedException();
        }

        public void ProcessMessage(HandlerContext<MockRegisterOrderCommand> context, EventStream stream)
        {
            throw new NotImplementedException();
        }
    }
}