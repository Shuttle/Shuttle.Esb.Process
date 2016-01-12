namespace Shuttle.ESB.Process
{
    public interface IProcessConfiguration
    {
        string ProviderName { get; set; }
        string ConnectionString { get; set;  }
        IProcessActivator ProcessActivator { get; set; }
        IProcessFactory ProcessFactory { get; set; }
    }
}