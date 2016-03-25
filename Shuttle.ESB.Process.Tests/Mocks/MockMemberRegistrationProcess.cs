using System;

namespace Shuttle.Esb.Process.Tests
{
    public class MockMemberRegistrationProcess :
        IProcessManager,
        IProcessStartMessageHandler<MockRegisterMemberCommand>,
        IProcessMessageHandler<MockEMailSentEvent>,
        IProcessMessageHandler<MockCompleteMemberRegistrationCommand>
    {
        public Guid CorrelationId { get; set; }

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