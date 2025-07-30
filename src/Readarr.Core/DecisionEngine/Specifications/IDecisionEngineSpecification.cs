using Readarr.Core.IndexerSearch.Definitions;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.DecisionEngine.Specifications
{
    public interface IDecisionEngineSpecification
    {
        SpecificationPriority Priority { get; }
        RejectionType Type { get; }

        Decision IsSatisfiedBy(RemoteBook remoteBook, SearchCriteriaBase searchCriteria);
    }
}