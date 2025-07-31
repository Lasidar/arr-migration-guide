using System;
using System.Collections.Generic;
using FluentValidation.Results;
using NLog;
using Readarr.Common.Extensions;
using Readarr.Core.Configuration;
using Readarr.Core.Localization;
using Readarr.Core.Parser;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.ImportLists.BookTracker
{
    public class BookTrackerImport : BookImportListBase<BookTrackerSettings>
    {
        private readonly IBookTrackerProxy _bookTrackerProxy;
        
        public override string Name => "Book Tracker";
        public override ImportListType ListType => ImportListType.Other;
        public override TimeSpan MinRefreshInterval => TimeSpan.FromHours(12);

        public BookTrackerImport(IBookTrackerProxy bookTrackerProxy,
                                IImportListStatusService importListStatusService,
                                IConfigService configService,
                                IParsingService parsingService,
                                ILocalizationService localizationService,
                                Logger logger)
            : base(importListStatusService, configService, parsingService, localizationService, logger)
        {
            _bookTrackerProxy = bookTrackerProxy;
        }

        public override ImportListBookFetchResult Fetch()
        {
            var books = new List<ImportListBookInfo>();
            var anyFailure = false;

            try
            {
                var remoteBooks = _bookTrackerProxy.GetUserList(Settings);

                foreach (var book in remoteBooks)
                {
                    books.Add(new ImportListBookInfo
                    {
                        Title = book.Title,
                        AuthorName = book.AuthorName,
                        Isbn = book.Isbn,
                        Asin = book.Asin,
                        GoodreadsId = book.GoodreadsId,
                        Year = book.Year,
                        Publisher = book.Publisher,
                        ReleaseDate = book.ReleaseDate,
                        Edition = book.Edition
                    });
                }

                _importListStatusService.RecordSuccess(Definition.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch books from Book Tracker");
                _importListStatusService.RecordFailure(Definition.Id);
                anyFailure = true;
            }

            return new ImportListBookFetchResult(CleanupListItems(books), anyFailure);
        }

        protected override void Test(List<ValidationFailure> failures)
        {
            failures.AddIfNotNull(_bookTrackerProxy.Test(Settings));
        }
    }
}