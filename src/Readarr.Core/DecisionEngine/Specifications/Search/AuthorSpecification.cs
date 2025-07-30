using NLog;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.DecisionEngine.Specifications.Search
{
    public class AuthorSpecification : IBookDownloadDecisionEngineSpecification
    {
        private readonly Logger _logger;

        public AuthorSpecification(Logger logger)
        {
            _logger = logger;
        }

        public SpecificationPriority Priority => SpecificationPriority.Default;
        public RejectionType Type => RejectionType.Permanent;

        public DownloadSpecDecision IsSatisfiedBy(RemoteBook subject, ReleaseDecisionInformation information)
        {
            if (information.SearchCriteria == null)
            {
                return DownloadSpecDecision.Accept();
            }

            _logger.Debug("Checking if author matches searched author");

            if (subject.Author.Id != information.SearchCriteria.Author.Id)
            {
                _logger.Debug("Author {0} does not match {1}", subject.Author.Name, information.SearchCriteria.Author.Name);
                return DownloadSpecDecision.Reject("Wrong author");
            }

            return DownloadSpecDecision.Accept();
        }
    }
}