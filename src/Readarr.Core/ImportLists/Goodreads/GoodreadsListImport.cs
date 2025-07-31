using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using NLog;
using Readarr.Common.Extensions;
using Readarr.Core.Configuration;
using Readarr.Core.Localization;
using Readarr.Core.Parser;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.ImportLists.Goodreads
{
    public class GoodreadsListImport : BookImportListBase<GoodreadsListSettings>
    {
        private readonly IGoodreadsProxy _goodreadsProxy;
        
        public override string Name => "Goodreads List";
        public override ImportListType ListType => ImportListType.Goodreads;
        public override TimeSpan MinRefreshInterval => TimeSpan.FromHours(6);

        public GoodreadsListImport(IGoodreadsProxy goodreadsProxy,
                                   IImportListStatusService importListStatusService,
                                   IConfigService configService,
                                   IParsingService parsingService,
                                   ILocalizationService localizationService,
                                   Logger logger)
            : base(importListStatusService, configService, parsingService, localizationService, logger)
        {
            _goodreadsProxy = goodreadsProxy;
        }

        public override ImportListBookFetchResult Fetch()
        {
            var books = new List<ImportListBookInfo>();
            var anyFailure = false;

            try
            {
                var remoteBooks = _goodreadsProxy.GetListBooks(Settings.ListId, Settings.ApiKey);

                foreach (var book in remoteBooks)
                {
                    books.Add(new ImportListBookInfo
                    {
                        Title = book.Title,
                        AuthorName = book.AuthorName,
                        GoodreadsId = book.GoodreadsId,
                        Isbn = book.Isbn,
                        Year = book.PublicationYear ?? 0,
                        Publisher = book.Publisher,
                        ReleaseDate = book.PublicationDate
                    });
                }

                _importListStatusService.RecordSuccess(Definition.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch books from Goodreads list");
                _importListStatusService.RecordFailure(Definition.Id);
                anyFailure = true;
            }

            return new ImportListBookFetchResult(CleanupListItems(books), anyFailure);
        }

        protected override void Test(List<ValidationFailure> failures)
        {
            failures.AddIfNotNull(_goodreadsProxy.Test(Settings));
        }
    }
}