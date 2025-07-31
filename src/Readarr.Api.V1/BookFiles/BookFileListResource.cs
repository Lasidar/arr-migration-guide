using System.Collections.Generic;
using Readarr.Core.Languages;
using Readarr.Core.Qualities;

namespace Readarr.Api.V1.BookFiles
{
    public class BookFileListResource
    {
        public List<int> BookFileIds { get; set; }
        public QualityModel Quality { get; set; }
        public List<Language> Languages { get; set; }
        public string ReleaseGroup { get; set; }
        public string SceneName { get; set; }
    }
}