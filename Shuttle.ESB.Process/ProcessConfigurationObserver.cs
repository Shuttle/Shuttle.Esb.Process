using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;
using Shuttle.ESB.Core;
using Shuttle.Recall.Core;

namespace Shuttle.Esb.Process
{
    public class ProcessConfigurationObserver : IPipelineObserver<OnInitializing>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IEventStore _eventStore;
        private readonly IKeyStore _keyStore;
        private readonly IProcessConfiguration _configuration;

        public ProcessConfigurationObserver(IDatabaseContextFactory databaseContextFactory, IEventStore eventStore, IKeyStore keyStore, IProcessConfiguration configuration)
        {
            Guard.AgainstNull(databaseContextFactory, "databaseContextFactory");
            Guard.AgainstNull(eventStore, "eventStore");
            Guard.AgainstNull(keyStore, "keyStore");
            Guard.AgainstNull(configuration, "configuration");

            _databaseContextFactory = databaseContextFactory;
            _eventStore = eventStore;
            _keyStore = keyStore;
            _configuration = configuration;
        }

        public void Execute(OnInitializing pipelineEvent)
        {
            pipelineEvent.Pipeline.State.GetServiceBus().Configuration.MessageHandlerInvoker =
                new ProcessMessageHandlerInvoker(_databaseContextFactory, _eventStore, _keyStore, _configuration);
        }
    }
}