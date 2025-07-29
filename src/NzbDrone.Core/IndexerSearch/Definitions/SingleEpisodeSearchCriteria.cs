namespace NzbDrone.Core.IndexerSearch.Definitions
{
    public class SingleEpisodeSearchCriteria : SearchCriteriaBase
    {
        public int EditionNumber { get; set; }
        public int BookNumber { get; set; }

        public override string ToString()
        {
            return string.Format("[{0} : S{1:00}E{2:00}]", Series.Title, BookNumber, EditionNumber);
        }
    }
}
