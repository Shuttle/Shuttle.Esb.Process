using System;

namespace Shuttle.Esb.Process.Tests
{
	public class MockMemberRegistrationProcess :
		IProcessManager,
		IProcessStartMessageHandler<MockRegisterMember>,
		IProcessMessageHandler<MockEMailSent>,
		IProcessMessageHandler<MockCompleteMemberRegistration>
	{
		public Guid CorrelationId { get; set; }

		public void ProcessMessage(IProcessHandlerContext<MockCompleteMemberRegistration> context)
		{
			throw new NotImplementedException();
		}

		public void ProcessMessage(IProcessHandlerContext<MockEMailSent> context)
		{
			throw new NotImplementedException();
		}

		public void ProcessMessage(IProcessHandlerContext<MockRegisterMember> context)
		{
			throw new NotImplementedException();
		}
	}
}