namespace NzbDrone.Core.IndexerSearch.Definitions
{
    public class BookSearchCriteria : SearchCriteriaBase
    {
        public int BookNumber { get; set; }

        public override string ToString()
        {
            return string.Format("[{0} : S{1:00}]", Series.Title, BookNumber);
        }
    }
}
