namespace NzbDrone.Core.IndexerSearch.Definitions
{
    public class AnimeSeasonSearchCriteria : SearchCriteriaBase
    {
        public int BookNumber { get; set; }

        public override string ToString()
        {
            return $"[{Series.Title} : S{BookNumber:00}]";
        }
    }
}
