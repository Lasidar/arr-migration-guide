using System;
using System.Collections.Generic;
using Readarr.Api.V1.Author;
using Readarr.Api.V1.Book;
using Readarr.Core.CustomFormats;
using Readarr.Core.History;
using Readarr.Core.Languages;
using Readarr.Core.Qualities;
using Readarr.Api.V3.CustomFormats;
using Readarr.Http.REST;

namespace Readarr.Api.V1.History
{
    public class BookHistoryResource : RestResource
    {
        public int BookId { get; set; }
        public int AuthorId { get; set; }
        public string SourceTitle { get; set; }
        public List<Language> Languages { get; set; }
        public QualityModel Quality { get; set; }
        public List<CustomFormatResource> CustomFormats { get; set; }
        public int CustomFormatScore { get; set; }
        public bool QualityCutoffNotMet { get; set; }
        public DateTime Date { get; set; }
        public string DownloadId { get; set; }

        public BookHistoryEventType EventType { get; set; }

        public Dictionary<string, string> Data { get; set; }

        public BookResource Book { get; set; }
        public AuthorResource Author { get; set; }
    }

    public enum BookHistoryEventType
    {
        Unknown = 0,
        Grabbed = 1,
        AuthorFolderImported = 2,
        BookFileImported = 3,
        DownloadFailed = 4,
        BookFileDeleted = 5,
        BookFileRenamed = 6,
        BookImportIncomplete = 7,
        DownloadImported = 8,
        BookFileRetagged = 9,
        DownloadIgnored = 10
    }

    public static class BookHistoryResourceMapper
    {
        public static BookHistoryResource ToResource(this EntityHistory model, Core.Books.Book book, Core.Books.Author author, ICustomFormatCalculationService formatCalculationService)
        {
            if (model == null)
            {
                return null;
            }

            var customFormats = formatCalculationService.ParseCustomFormat(model, author);
            var customFormatScore = author.QualityProfile?.Value?.CalculateCustomFormatScore(customFormats) ?? 0;

            return new BookHistoryResource
            {
                Id = model.Id,
                BookId = model.BookId,
                AuthorId = model.AuthorId,
                SourceTitle = model.SourceTitle,
                Languages = model.Languages,
                Quality = model.Quality,
                CustomFormats = customFormats?.ToResource(false),
                CustomFormatScore = customFormatScore,
                QualityCutoffNotMet = model.QualityCutoffNotMet,
                Date = model.Date,
                DownloadId = model.DownloadId,
                EventType = (BookHistoryEventType)model.EventType,
                Data = model.Data,
                Book = book.ToResource(),
                Author = author.ToResource()
            };
        }
    }
}