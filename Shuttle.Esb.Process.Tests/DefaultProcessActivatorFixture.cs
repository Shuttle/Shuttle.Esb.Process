using System;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace Shuttle.Esb.Process.Tests
{
	[TestFixture]
	public class DefaultProcessActivatorFixture
	{
		[Test]
		public void Should_be_able_to_request_mapping_registration()
		{
			var transportMessage = new TransportMessage();

			var processManagementOptions = new ProcessManagementOptions();
			processManagementOptions.AssemblyNames.Add("Shuttle.Esb.Process.Tests");

			var activator = new ProcessActivator(Options.Create(processManagementOptions));

			Assert.IsFalse(activator.IsProcessMessage(transportMessage, new MockNullCommand()));

			Assert.IsTrue(activator.IsProcessMessage(transportMessage, new MockRegisterOrderCommand()));
			Assert.IsTrue(activator.IsProcessMessage(transportMessage, new MockEMailSentEvent()));
			Assert.IsTrue(activator.IsProcessMessage(transportMessage, new MockCompleteOrderCommand()));

			Assert.IsTrue(activator.IsProcessMessage(transportMessage, new MockRegisterMemberCommand()));
			Assert.IsTrue(activator.IsProcessMessage(transportMessage, new MockCompleteMemberRegistrationCommand()));
		}

		[Test]
		public void Should_be_able_to_resolve_message_to_multiple_processes()
		{
			var transportMessage = new TransportMessage
			{
				CorrelationId = Guid.NewGuid().ToString()
			};

			var processManagementOptions = new ProcessManagementOptions();
			processManagementOptions.AssemblyNames.Add("Shuttle.Esb.Process.Tests");

			var activator = new ProcessActivator(Options.Create(processManagementOptions));

			activator.RegisterResolver<MockEMailSentEvent>(
				(transport, message) => new MessageProcessType(typeof (MockOrderProcess), false));

			var instance = activator.Create(transportMessage, new MockEMailSentEvent());

			Assert.IsTrue(instance.GetType() == typeof (MockOrderProcess));
		}

		[Test]
		public void Should_be_able_to_start_process()
		{
			var transportMessage = new TransportMessage();

			var processManagementOptions = new ProcessManagementOptions();
			processManagementOptions.AssemblyNames.Add("Shuttle.Esb.Process.Tests");

			var activator = new ProcessActivator(Options.Create(processManagementOptions));

			Assert.IsTrue(typeof (MockOrderProcess) ==
			              activator.Create(transportMessage, new MockRegisterOrderCommand()).GetType());
			Assert.IsTrue(typeof (MockMemberRegistrationProcess) ==
			              activator.Create(transportMessage, new MockRegisterMemberCommand()).GetType());
		}

		[Test]
		public void Should_throw_exception_when_creating_unknown_process()
		{
			var transportMessage = new TransportMessage();

			var activator = new ProcessActivator(Options.Create(new ProcessManagementOptions()));

			Assert.Throws<ProcessException>(() => activator.Create(transportMessage, new MockRegisterMemberCommand()));
		}

		[Test]
		public void Should_throw_exception_when_no_resolver_for_message_to_multiple_processes()
		{
			var transportMessage = new TransportMessage();

			var processManagementOptions = new ProcessManagementOptions();
			processManagementOptions.AssemblyNames.Add("Shuttle.Esb.Process.Tests");

			var activator = new ProcessActivator(Options.Create(processManagementOptions));

			Assert.Throws<ProcessException>(() => activator.Create(transportMessage, new MockEMailSentEvent()));
		}
	}
}