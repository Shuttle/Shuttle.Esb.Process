namespace Shuttle.Esb.Process
{
	public interface IProcessConfiguration
	{
		string ProviderName { get; set; }
		string ConnectionString { get; set; }
	}
}