using Shuttle.Core.Pipelines;
using Shuttle.Core.Specification;

namespace Shuttle.Esb.Process
{
	public interface IProcessMessageAssessor : ISpecification<IPipelineEvent>
	{
	}
}