using System;
using System.Collections.Generic;
using System.Linq;
using Readarr.Core.Books;
using Readarr.Core.CustomFormats;
using Readarr.Core.Download.Clients;
using Readarr.Core.Languages;

namespace Readarr.Core.Parser.Model
{
    public class RemoteBook
    {
        public ReleaseInfo Release { get; set; }
        public BookInfo ParsedBookInfo { get; set; }
        public Author Author { get; set; }
        public List<Book> Books { get; set; }
        public bool DownloadAllowed { get; set; }
        public TorrentSeedConfiguration SeedConfiguration { get; set; }
        public int PreferredWordScore { get; set; }
        public List<Language> Languages { get; set; }
        public List<CustomFormat> CustomFormats { get; set; }
        public int CustomFormatScore { get; set; }

        public RemoteBook()
        {
            Books = new List<Book>();
            Languages = new List<Language>();
            CustomFormats = new List<CustomFormat>();
        }

        public bool IsRecentBook()
        {
            return Books.Any() && Books.First().Metadata.Value?.ReleaseDate >= DateTime.UtcNow.Date.AddDays(-14);
        }

        public override string ToString()
        {
            return Release.Title;
        }
    }
}