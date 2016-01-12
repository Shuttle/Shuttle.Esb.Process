using System;

namespace Shuttle.ESB.Process.Tests
{
    public class MockOrderProcess :
        IProcessManager,
        IProcessStartMessageHandler<MockRegisterOrderCommand>,
        IProcessMessageHandler<MockEMailSentEvent>,
        IProcessMessageHandler<MockCompleteOrderCommand>
    {
        public Guid CorrelationId { get; set; }

        public void ProcessMessage(IProcessHandlerContext<MockCompleteOrderCommand> context)
        {
            throw new NotImplementedException();
        }

        public void ProcessMessage(IProcessHandlerContext<MockEMailSentEvent> context)
        {
            throw new NotImplementedException();
        }

        public void ProcessMessage(IProcessHandlerContext<MockRegisterOrderCommand> context)
        {
            throw new NotImplementedException();
        }
    }
}