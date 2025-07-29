using System.Collections.Generic;
using NzbDrone.Core.Languages;
using NzbDrone.Core.Qualities;

namespace Readarr.Api.V3.EditionFiles
{
    public class EditionFileListResource
    {
        public List<int> EditionFileIds { get; set; }
        public List<Language> Languages { get; set; }
        public QualityModel Quality { get; set; }
        public string SceneName { get; set; }
        public string ReleaseGroup { get; set; }
    }
}
