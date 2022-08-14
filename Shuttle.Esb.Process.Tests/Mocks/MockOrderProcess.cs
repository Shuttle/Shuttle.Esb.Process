using System;

namespace Shuttle.Esb.Process.Tests
{
	public class MockOrderProcess :
		IProcessManager,
		IProcessStartMessageHandler<MockRegisterOrder>,
		IProcessMessageHandler<MockEMailSent>,
		IProcessMessageHandler<MockCompleteOrder>
	{
		public Guid CorrelationId { get; set; }

		public void ProcessMessage(IProcessHandlerContext<MockCompleteOrder> context)
		{
			throw new NotImplementedException();
		}

		public void ProcessMessage(IProcessHandlerContext<MockEMailSent> context)
		{
			throw new NotImplementedException();
		}

		public void ProcessMessage(IProcessHandlerContext<MockRegisterOrder> context)
		{
			throw new NotImplementedException();
		}
	}
}