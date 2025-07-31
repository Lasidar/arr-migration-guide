using System.Collections.Generic;
using Readarr.Core.Languages;
using Readarr.Core.Parser.Model;
using Readarr.Core.Books;
using Readarr.Core.Tv;

namespace Readarr.Core.CustomFormats
{
    public class CustomFormatInput
    {
        public ParsedEpisodeInfo EpisodeInfo { get; set; }
        public Tv.Series Series { get; set; }
        public long Size { get; set; }
        public IndexerFlags IndexerFlags { get; set; }
        public List<Language> Languages { get; set; }
        public string Filename { get; set; }
        public ReleaseType ReleaseType { get; set; }

        public CustomFormatInput()
        {
            Languages = new List<Language>();
        }

        // public CustomFormatInput(ParsedEpisodeInfo episodeInfo, Tv.Series series)
        // {
        //     EpisodeInfo = episodeInfo;
        //     Series = series;
        // }
        //
        // public CustomFormatInput(ParsedEpisodeInfo episodeInfo, Tv.Series series, long size, List<Language> languages)
        // {
        //     EpisodeInfo = episodeInfo;
        //     Series = series;
        //     Size = size;
        //     Languages = languages;
        // }
        //
        // public CustomFormatInput(ParsedEpisodeInfo episodeInfo, Tv.Series series, long size, List<Language> languages, string filename)
        // {
        //     EpisodeInfo = episodeInfo;
        //     Series = series;
        //     Size = size;
        //     Languages = languages;
        //     Filename = filename;
        // }
    }
}
