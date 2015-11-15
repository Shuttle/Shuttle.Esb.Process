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

        public void ProcessMessage(ProcessHandlerContext<MockCompleteMemberRegistrationCommand> context)
        {
            throw new NotImplementedException();
        }

        public void ProcessMessage(ProcessHandlerContext<MockEMailSentEvent> context)
        {
            throw new NotImplementedException();
        }

        public void ProcessMessage(ProcessHandlerContext<MockRegisterMemberCommand> context)
        {
            throw new NotImplementedException();
        }
    }
}