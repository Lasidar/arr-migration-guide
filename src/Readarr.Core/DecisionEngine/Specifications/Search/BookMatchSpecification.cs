using System.Linq;
using NLog;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.DecisionEngine.Specifications.Search
{
    public class BookMatchSpecification : IBookDownloadDecisionEngineSpecification
    {
        private readonly Logger _logger;

        public BookMatchSpecification(Logger logger)
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

            var searchedBooks = information.SearchCriteria.Books;
            if (!searchedBooks.Any())
            {
                return DownloadSpecDecision.Accept();
            }

            var matchingBooks = subject.Books.Intersect(searchedBooks).ToList();
            
            _logger.Debug("Checking if books match searched books. {0} matched books", matchingBooks.Count);

            if (!matchingBooks.Any())
            {
                _logger.Debug("Books in release do not match searched books");
                return DownloadSpecDecision.Reject("Wrong books");
            }

            return DownloadSpecDecision.Accept();
        }
    }
}