using System.Collections.Generic;
using NzbDrone.Core.Languages;
using NzbDrone.Core.Parser.Model;
using NzbDrone.Core.Qualities;
using Readarr.Api.V3.CustomFormats;
using Readarr.Api.V3.Episodes;
using Readarr.Http.REST;

namespace Readarr.Api.V3.ManualImport
{
    public class ManualImportReprocessResource : RestResource
    {
        public string Path { get; set; }
        public int AuthorId { get; set; }
        public int? BookNumber { get; set; }
        public List<EpisodeResource> Episodes { get; set; }
        public List<int> EpisodeIds { get; set; }
        public QualityModel Quality { get; set; }
        public List<Language> Languages { get; set; }
        public string ReleaseGroup { get; set; }
        public string DownloadId { get; set; }
        public List<CustomFormatResource> CustomFormats { get; set; }
        public int CustomFormatScore { get; set; }
        public int IndexerFlags { get; set; }
        public ReleaseType ReleaseType { get; set; }
        public IEnumerable<ImportRejectionResource> Rejections { get; set; }
    }
}
