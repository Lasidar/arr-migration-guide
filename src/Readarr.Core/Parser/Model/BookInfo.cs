using System;
using System.Collections.Generic;
using Readarr.Core.Languages;
using Readarr.Core.Qualities;

namespace Readarr.Core.Parser.Model
{
    public class BookInfo
    {
        public string BookTitle { get; set; }
        public string AuthorName { get; set; }
        public string AuthorTitleInfo { get; set; }
        public string BookSubtitle { get; set; }
        public int BookYear { get; set; }
        public string Isbn { get; set; }
        public string Asin { get; set; }
        public string GoodreadsId { get; set; }
        public string AuthorMBId { get; set; }
        public string BookMBId { get; set; }
        public string ReleaseMBId { get; set; }
        public string RecordingMBId { get; set; }
        public string TrackMBId { get; set; }
        public int DiscNumber { get; set; }
        public int DiscCount { get; set; }
        public QualityModel Quality { get; set; }
        public string ReleaseDate { get; set; }
        public string ReleaseGroup { get; set; }
        public string ReleaseHash { get; set; }
        public string ReleaseVersion { get; set; }
        public ReleaseType ReleaseType { get; set; }
        public List<Language> Languages { get; set; }

        public BookInfo()
        {
            Languages = new List<Language>();
        }

        public override string ToString()
        {
            var bookString = string.Empty;

            if (!string.IsNullOrWhiteSpace(AuthorName))
            {
                bookString += AuthorName;
            }

            if (!string.IsNullOrWhiteSpace(BookTitle))
            {
                bookString += string.Format(" - {0}", BookTitle);
            }

            if (BookYear > 0)
            {
                bookString += string.Format(" ({0})", BookYear);
            }

            if (!string.IsNullOrWhiteSpace(ReleaseGroup))
            {
                bookString += string.Format(" [{0}]", ReleaseGroup);
            }

            return bookString.Trim();
        }
    }

    public enum ReleaseType
    {
        Unknown = 0,
        SingleBook = 1,
        MultiBook = 2,
        Anthology = 3
    }
}