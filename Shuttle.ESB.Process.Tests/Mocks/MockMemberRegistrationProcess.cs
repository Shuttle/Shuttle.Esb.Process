using System;

namespace Shuttle.ESB.Process.Tests
{
    public class MockMemberRegistrationProcess :
        IProcessManager,
        IProcessStartMessageHandler<MockRegisterMemberCommand>,
        IProcessMessageHandler<MockEMailSentEvent>,
        IProcessMessageHandler<MockCompleteMemberRegistrationCommand>
    {
        public MockMemberRegistrationProcess(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; private set; }

        public void ProcessMessage(IProcessHandlerContext<MockCompleteMemberRegistrationCommand> context)
        {
            throw new NotImplementedException();
        }

        public void ProcessMessage(IProcessHandlerContext<MockEMailSentEvent> context)
        {
            throw new NotImplementedException();
        }

        public void ProcessMessage(IProcessHandlerContext<MockRegisterMemberCommand> context)
        {
            throw new NotImplementedException();
        }
    }
}