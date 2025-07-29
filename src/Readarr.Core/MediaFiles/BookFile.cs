using System;
using System.Collections.Generic;
using Readarr.Common.Extensions;
using Readarr.Core.Datastore;
using Readarr.Core.Languages;
using Readarr.Core.MediaFiles.MediaInfo;
using Readarr.Core.Parser.Model;
using Readarr.Core.Qualities;
using Readarr.Core.Books;

namespace Readarr.Core.MediaFiles
{
    public class BookFile : ModelBase
    {
        public int AuthorId { get; set; }
        public int BookId { get; set; }
        public string RelativePath { get; set; }
        public string Path { get; set; }
        public long Size { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime Modified { get; set; }
        public string OriginalFilePath { get; set; }
        public string SceneName { get; set; }
        public string ReleaseGroup { get; set; }
        public string ReleaseHash { get; set; }
        public QualityModel Quality { get; set; }
        public IndexerFlags IndexerFlags { get; set; }
        public MediaInfoModel MediaInfo { get; set; }
        public int EditionId { get; set; }
        public string CalibreId { get; set; }
        public int Part { get; set; } // For multi-part books
        
        // Relationships
        public LazyLoaded<Edition> Edition { get; set; }
        public LazyLoaded<Author> Author { get; set; }
        public LazyLoaded<Book> Book { get; set; }
        public List<Language> Languages { get; set; }
        public ReleaseType ReleaseType { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}] {1}", Id, RelativePath);
        }

        public string GetSceneOrFileName()
        {
            if (SceneName.IsNotNullOrWhiteSpace())
            {
                return SceneName;
            }

            if (RelativePath.IsNotNullOrWhiteSpace())
            {
                return System.IO.Path.GetFileNameWithoutExtension(RelativePath);
            }

            if (Path.IsNotNullOrWhiteSpace())
            {
                return System.IO.Path.GetFileNameWithoutExtension(Path);
            }

            return string.Empty;
        }
    }
}