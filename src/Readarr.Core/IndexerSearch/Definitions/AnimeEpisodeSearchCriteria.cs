namespace Readarr.Core.IndexerSearch.Definitions
{
    public class AnimeEpisodeSearchCriteria : SearchCriteriaBase
    {
        public int AbsoluteEpisodeNumber { get; set; }
        public int SeasonNumber { get; set; }
        public int EpisodeNumber { get; set; }

        public override string ToString()
        {
            return "[Anime Episode Search]";
        }
    }
}