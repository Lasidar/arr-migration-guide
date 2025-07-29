using System.Collections.Generic;
using Readarr.Core.Languages;
using Readarr.Core.Qualities;

namespace Readarr.Api.V1.EpisodeFiles
{
    public class EpisodeFileListResource
    {
        public List<int> EpisodeFileIds { get; set; }
        public List<Language> Languages { get; set; }
        public QualityModel Quality { get; set; }
        public string SceneName { get; set; }
        public string ReleaseGroup { get; set; }
    }
}
