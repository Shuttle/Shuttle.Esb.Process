using System.Configuration;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Esb.Process
{
	public class ProcessSection : ConfigurationSection
	{
		[ConfigurationProperty("connectionStringName", IsRequired = false, DefaultValue = "Process")]
		public string ConnectionStringName
		{
			get { return (string) this["connectionStringName"]; }
		}

		public static ProcessConfiguration Configuration()
		{
			var section = ConfigurationSectionProvider.Open<ProcessSection>("shuttle", "process");
			var configuration = new ProcessConfiguration();

			var connectionStringName = "Process";

			if (section != null)
			{
				connectionStringName = section.ConnectionStringName;
			}

			var settings = ConfigurationManager.ConnectionStrings[connectionStringName];

			configuration.ConnectionString = settings.ConnectionString;
			configuration.ProviderName = settings.ProviderName;

			return configuration;
		}
	}
}