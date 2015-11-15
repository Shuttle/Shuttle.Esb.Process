using System;

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

        public void ProcessMessage(ProcessHandlerContext<MockCompleteOrderCommand> context)
        {
            throw new NotImplementedException();
        }

        public void ProcessMessage(ProcessHandlerContext<MockEMailSentEvent> context)
        {
            throw new NotImplementedException();
        }

        public void ProcessMessage(ProcessHandlerContext<MockRegisterOrderCommand> context)
        {
            throw new NotImplementedException();
        }
    }
}