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

			Assert.IsTrue(activator.IsProcessMessage(transportMessage, new MockRegisterOrder()));
			Assert.IsTrue(activator.IsProcessMessage(transportMessage, new MockEMailSent()));
			Assert.IsTrue(activator.IsProcessMessage(transportMessage, new MockCompleteOrder()));

			Assert.IsTrue(activator.IsProcessMessage(transportMessage, new MockRegisterMember()));
			Assert.IsTrue(activator.IsProcessMessage(transportMessage, new MockCompleteMemberRegistration()));
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

			activator.RegisterResolver<MockEMailSent>(
				(transport, message) => new MessageProcessType(typeof (MockOrderProcess), false));

			var instance = activator.Create(transportMessage, new MockEMailSent());

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
			              activator.Create(transportMessage, new MockRegisterOrder()).GetType());
			Assert.IsTrue(typeof (MockMemberRegistrationProcess) ==
			              activator.Create(transportMessage, new MockRegisterMember()).GetType());
		}

		[Test]
		public void Should_throw_exception_when_creating_unknown_process()
		{
			var transportMessage = new TransportMessage();

			var activator = new ProcessActivator(Options.Create(new ProcessManagementOptions()));

			Assert.Throws<ProcessException>(() => activator.Create(transportMessage, new MockNullCommand()));
		}

		[Test]
		public void Should_throw_exception_when_no_resolver_for_message_to_multiple_processes()
		{
			var transportMessage = new TransportMessage();

			var processManagementOptions = new ProcessManagementOptions();
			processManagementOptions.AssemblyNames.Add("Shuttle.Esb.Process.Tests");

			var activator = new ProcessActivator(Options.Create(processManagementOptions));

			Assert.Throws<ProcessException>(() => activator.Create(transportMessage, new MockEMailSent()));
		}
	}
}