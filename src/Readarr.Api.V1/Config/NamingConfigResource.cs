using Readarr.Http.REST;
using Readarr.Core.Tv;

namespace Readarr.Api.V1.Config
{
    public class NamingConfigResource : RestResource
    {
        public bool RenameEpisodes { get; set; }
        public bool ReplaceIllegalCharacters { get; set; }
        public int ColonReplacementFormat { get; set; }
        public string CustomColonReplacementFormat { get; set; }
        public int MultiEpisodeStyle { get; set; }
        public string StandardEpisodeFormat { get; set; }
        public string DailyEpisodeFormat { get; set; }
        public string AnimeEpisodeFormat { get; set; }
        public string SeriesFolderFormat { get; set; }
        public string SeasonFolderFormat { get; set; }
        public string SpecialsFolderFormat { get; set; }
    }
}
