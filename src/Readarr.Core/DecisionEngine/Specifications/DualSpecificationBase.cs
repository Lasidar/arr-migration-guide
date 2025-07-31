using Readarr.Core.Parser.Model;
using Readarr.Core.Tv;

namespace Readarr.Core.DecisionEngine.Specifications
{
    // Abstract base class that provides default implementations for dual specifications
    public abstract class DualSpecificationBase : IDualDownloadDecisionEngineSpecification
    {
        public abstract RejectionType Type { get; }
        public abstract SpecificationPriority Priority { get; }

        // TV method - default implementation delegates to book method if possible
        public virtual DownloadSpecDecision IsSatisfiedBy(RemoteEpisode subject, ReleaseDecisionInformation information)
        {
            // Default: reject TV content in a book-focused application
            return new DownloadSpecDecision(new Rejection("TV content is not supported in Readarr"));
        }

        // Book method - must be implemented by derived classes
        public abstract DownloadSpecDecision IsSatisfiedBy(RemoteBook subject, ReleaseDecisionInformation information);
    }
}