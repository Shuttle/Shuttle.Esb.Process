using System;
using Shuttle.ESB.Core;
using Shuttle.Recall.Core;

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

        public void ProcessMessage(HandlerContext<MockCompleteMemberRegistrationCommand> context, EventStream stream)
        {
            throw new NotImplementedException();
        }

        public void ProcessMessage(HandlerContext<MockEMailSentEvent> context, EventStream stream)
        {
            throw new NotImplementedException();
        }

        public void ProcessMessage(HandlerContext<MockRegisterMemberCommand> context, EventStream stream)
        {
            throw new NotImplementedException();
        }
    }
}