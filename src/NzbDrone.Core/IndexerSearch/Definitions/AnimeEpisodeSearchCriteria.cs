namespace NzbDrone.Core.IndexerSearch.Definitions
{
    public class AnimeEpisodeSearchCriteria : SearchCriteriaBase
    {
        public int AbsoluteEditionNumber { get; set; }
        public int EditionNumber { get; set; }
        public int BookNumber { get; set; }
        public bool IsSeasonSearch { get; set; }

        public override string ToString()
        {
            return $"[{Series.Title} : S{BookNumber:00}E{EditionNumber:00} ({AbsoluteEditionNumber:00})]";
        }
    }
}
