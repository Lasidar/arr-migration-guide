namespace Readarr.Core.IndexerSearch.Definitions
{
    public class IsbnSearchCriteria : SearchCriteriaBase
    {
        public string Isbn { get; set; }
        public string CleanIsbn => Isbn?.Replace("-", "").Replace(" ", "");

        public override string ToString()
        {
            return $"ISBN: {Isbn}";
        }
    }
}