using Shuttle.Core.Pipelines;
using Shuttle.Core.Specification;

namespace Shuttle.Esb.Process
{
	public interface IProcessMessageSpecification : ISpecification<IPipelineEvent>
	{
	}
}