namespace Readarr.Core.MetadataSource.BookInfo.Resources
{
    public class ImageResource
    {
        public string Url { get; set; }
        public string CoverType { get; set; }
    }

    public class LinkResource
    {
        public string Url { get; set; }
        public string Name { get; set; }
    }

    public class RatingsResource
    {
        public int Count { get; set; }
        public decimal Value { get; set; }
        public double? Popularity { get; set; }
    }

    public class SeriesResource
    {
        public string ForeignSeriesId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Numbered { get; set; }
    }

    public class SeriesLinkResource
    {
        public int SeriesId { get; set; }
        public string Position { get; set; }
    }
}