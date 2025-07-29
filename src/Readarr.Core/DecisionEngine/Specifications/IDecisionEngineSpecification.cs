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

    public enum SpecificationPriority
    {
        Default = 0,
        Parsing = 1,
        Database = 2,
        Search = 3,
        Quality = 4,
        Cutoff = 5
    }
}