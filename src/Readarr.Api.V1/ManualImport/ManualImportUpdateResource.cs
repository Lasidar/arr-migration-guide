using System.Collections.Generic;
using Readarr.Core.Languages;
using Readarr.Core.Parser.Model;
using Readarr.Core.Qualities;
using Readarr.Http.REST;

namespace Readarr.Api.V1.ManualImport
{
    public class ManualImportUpdateResource : RestResource
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public int? AuthorId { get; set; }
        public int? BookId { get; set; }
        public QualityModel Quality { get; set; }
        public List<Language> Languages { get; set; }
        public string ReleaseGroup { get; set; }
        public string DownloadId { get; set; }
        public bool AdditionalFile { get; set; }
        public bool ReplaceExistingFiles { get; set; }
        public bool DisableReleaseSwitching { get; set; }
        public IndexerFlags IndexerFlags { get; set; }
        public ReleaseType ReleaseType { get; set; }
    }
}