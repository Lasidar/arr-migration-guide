namespace Readarr.Core.IndexerSearch.Definitions
{
    public class AuthorSearchCriteria : SearchCriteriaBase
    {
        public string AuthorQuery { get; set; }
        public string AuthorName { get; set; }
        public string GoodreadsId { get; set; }

        public override string ToString()
        {
            return $"[{AuthorName}]";
        }
    }
}