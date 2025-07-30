using System.Collections.Generic;
using System.Linq;
using Readarr.Common.Extensions;
using Readarr.Core.Books;
using Readarr.Core.CustomFormats;
using Readarr.Core.Download;
using Readarr.Core.Languages;
using Readarr.Core.MediaFiles;
using Readarr.Core.MediaFiles.MediaInfo;
using Readarr.Core.Qualities;

namespace Readarr.Core.Parser.Model
{
    public class LocalBook
    {
        public LocalBook()
        {
            Books = new List<Book>();
            Languages = new List<Language>();
            CustomFormats = new List<CustomFormat>();
        }

        public string Path { get; set; }
        public long Size { get; set; }
        public BookInfo FileBookInfo { get; set; }
        public BookInfo DownloadClientBookInfo { get; set; }
        public DownloadClientItem DownloadItem { get; set; }
        public BookInfo FolderBookInfo { get; set; }
        public Author Author { get; set; }
        public List<Book> Books { get; set; }
        public List<DeletedBookFile> OldFiles { get; set; }
        public QualityModel Quality { get; set; }
        public List<Language> Languages { get; set; }
        public IndexerFlags IndexerFlags { get; set; }
        public ReleaseType ReleaseType { get; set; }
        public MediaInfoModel MediaInfo { get; set; }
        public bool ExistingFile { get; set; }
        public bool SceneSource { get; set; }
        public string ReleaseGroup { get; set; }
        public string ReleaseHash { get; set; }
        public string SceneName { get; set; }
        public List<CustomFormat> CustomFormats { get; set; }
        public int CustomFormatScore { get; set; }
        public GrabbedReleaseInfo Release { get; set; }
        public bool ScriptImported { get; set; }
        public string FileNameBeforeRename { get; set; }
        public SubtitleTitleInfo SubtitleInfo { get; set; }

        public override string ToString()
        {
            return Path;
        }
    }
}