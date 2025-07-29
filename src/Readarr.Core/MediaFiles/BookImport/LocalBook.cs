using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Readarr.Common.Disk;
using Readarr.Core.Books;
using Readarr.Core.Languages;
using Readarr.Core.MediaFiles.MediaInfo;
using Readarr.Core.Parser.Model;
using Readarr.Core.Qualities;

namespace Readarr.Core.MediaFiles.BookImport
{
    public class LocalBook
    {
        public LocalBook()
        {
            Tags = new List<string>();
        }

        public string Path { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public DateTime Modified { get; set; }
        public FileInfo FileInfo { get; set; }
        public BookInfo BookInfo { get; set; }
        public Author Author { get; set; }
        public Book Book { get; set; }
        public Edition Edition { get; set; }
        public List<Book> Books { get; set; }
        public QualityModel Quality { get; set; }
        public List<Language> Languages { get; set; }
        public MediaInfoModel MediaInfo { get; set; }
        public bool ExistingFile { get; set; }
        public bool AdditionalFile { get; set; }
        public bool SceneSource { get; set; }
        public string ReleaseGroup { get; set; }
        public string SceneName { get; set; }
        public int PreferredWordScore { get; set; }
        public List<string> Tags { get; set; }

        // Import specific
        public bool ShouldImportExtra { get; set; }
        public string FileBookInfo { get; set; }

        public override string ToString()
        {
            return Path;
        }
    }
}