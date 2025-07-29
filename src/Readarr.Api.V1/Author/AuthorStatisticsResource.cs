using System;
using System.Collections.Generic;
using System.Linq;
using Readarr.Core.AuthorStats;

namespace Readarr.Api.V1.Author
{
    public class AuthorStatisticsResource
    {
        public int BookCount { get; set; }
        public int BookFileCount { get; set; }
        public int BookCountWithFiles { get; set; }
        public long SizeOnDisk { get; set; }
        public int TotalBookCount { get; set; }
        public int AvailableBookCount { get; set; }
        public int MonitoredBookCount { get; set; }

        public List<BookStatisticsResource> BookStatistics { get; set; }
    }

    public class BookStatisticsResource
    {
        public int BookId { get; set; }
        public int EditionCount { get; set; }
        public int EditionFileCount { get; set; }
        public int TotalEditionCount { get; set; }
        public long SizeOnDisk { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public DateTime? PreviousAiring { get; set; }
    }

    public static class AuthorStatisticsResourceMapper
    {
        public static AuthorStatisticsResource ToResource(this AuthorStatistics model)
        {
            if (model == null)
            {
                return null;
            }

            return new AuthorStatisticsResource
            {
                BookCount = model.BookCount,
                BookFileCount = model.BookFileCount,
                BookCountWithFiles = model.BookCountWithFiles,
                SizeOnDisk = model.SizeOnDisk,
                TotalBookCount = model.TotalBookCount,
                AvailableBookCount = model.AvailableBookCount,
                MonitoredBookCount = model.MonitoredBookCount,
                BookStatistics = model.BookStatistics.ToResource()
            };
        }

        public static List<BookStatisticsResource> ToResource(this List<BookStatistics> models)
        {
            return models?.Select(ToResource).ToList() ?? new List<BookStatisticsResource>();
        }

        public static BookStatisticsResource ToResource(this BookStatistics model)
        {
            if (model == null)
            {
                return null;
            }

            return new BookStatisticsResource
            {
                BookId = model.BookId,
                EditionCount = model.EditionCount,
                EditionFileCount = model.EditionFileCount,
                TotalEditionCount = model.TotalEditionCount,
                SizeOnDisk = model.SizeOnDisk,
                ReleaseDate = model.ReleaseDate,
                PreviousAiring = model.PreviousAiring
            };
        }
    }
}