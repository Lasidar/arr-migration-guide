namespace Readarr.Api.V1.Books
{
    public class BookStatisticsResource
    {
        public int BookFileCount { get; set; }
        public int BookCount { get; set; }
        public int TotalBookCount { get; set; }
        public long SizeOnDisk { get; set; }
        public decimal PercentOfBooks { get; set; }
    }
}