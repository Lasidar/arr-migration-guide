using System.Collections.Generic;
using Readarr.Core.Qualities;

namespace Readarr.Core.Parser.Model
{
    public class BookInfo
    {
        public string BookTitle { get; set; }
        public string AuthorName { get; set; }
        public string AuthorTitle { get; set; }  // Combined author and title
        public string ReleaseTitle { get; set; }
        public int? Year { get; set; }
        public string Isbn { get; set; }
        public string Asin { get; set; }
        public string GoodreadsId { get; set; }
        public string Publisher { get; set; }
        public string ReleaseGroup { get; set; }
        public string ReleaseHash { get; set; }
        public QualityModel Quality { get; set; }
        public string BookFormat { get; set; }  // epub, mobi, pdf, etc.
        public List<string> ExtraInfo { get; set; }

        public BookInfo()
        {
            ExtraInfo = new List<string>();
        }

        public override string ToString()
        {
            return string.Format("{0} - {1} {2}", AuthorName, BookTitle, 
                Quality != null ? Quality.ToString() : "[Unknown Quality]");
        }
    }
}