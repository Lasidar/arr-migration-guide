using System.Collections.Generic;
using Readarr.Core.Languages;
using Readarr.Core.Parser.Model;
using Readarr.Api.V3.CustomFormats;
using Readarr.Api.V3.Episodes;
using Readarr.Api.V3.Series;
using Readarr.Http.REST;

namespace Readarr.Api.V1.Parse
{
    public class ParseResource : RestResource
    {
        public string Title { get; set; }
        public ParsedEpisodeInfo ParsedEpisodeInfo { get; set; }
        public SeriesResource Series { get; set; }
        public List<EpisodeResource> Episodes { get; set; }
        public List<Language> Languages { get; set; }
        public List<CustomFormatResource> CustomFormats { get; set; }
        public int CustomFormatScore { get; set; }
    }
}
