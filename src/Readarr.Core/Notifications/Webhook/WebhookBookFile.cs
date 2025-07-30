using System;
using System.Collections.Generic;
using Readarr.Core.Languages;
using Readarr.Core.MediaFiles;

namespace Readarr.Core.Notifications.Webhook
{
    public class WebhookBookFile
    {
        public WebhookBookFile()
        {
        }

        public WebhookBookFile(BookFile bookFile)
        {
            Id = bookFile.Id;
            Path = bookFile.Path;
            Quality = bookFile.Quality.Quality.Name;
            QualityVersion = bookFile.Quality.Revision.Version;
            ReleaseGroup = bookFile.ReleaseGroup;
            SceneName = bookFile.SceneName;
            Size = bookFile.Size;
            DateAdded = bookFile.DateAdded;
            Languages = bookFile.Languages;

            if (bookFile.MediaInfo != null)
            {
                MediaInfo = new WebhookBookFileMediaInfo(bookFile);
            }
        }

        public int Id { get; set; }
        public string Path { get; set; }
        public string Quality { get; set; }
        public int QualityVersion { get; set; }
        public string ReleaseGroup { get; set; }
        public string SceneName { get; set; }
        public long Size { get; set; }
        public DateTime DateAdded { get; set; }
        public List<Language> Languages { get; set; }
        public WebhookBookFileMediaInfo MediaInfo { get; set; }
        public string SourcePath { get; set; }
        public string RecycleBinPath { get; set; }
    }
}