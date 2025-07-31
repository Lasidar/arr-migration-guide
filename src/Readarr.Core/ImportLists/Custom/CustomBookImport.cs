using System;
using System.Collections.Generic;
using FluentValidation.Results;
using NLog;
using Readarr.Common.Extensions;
using Readarr.Core.Configuration;
using Readarr.Core.Localization;
using Readarr.Core.Parser;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.ImportLists.Custom
{
    public class CustomBookImport : BookImportListBase<CustomBookSettings>
    {
        private readonly ICustomBookImportProxy _customProxy;
        public override string Name => _localizationService.GetLocalizedString("ImportListsCustomListSettingsName");

        public override TimeSpan MinRefreshInterval => TimeSpan.FromHours(6);

        public override ImportListType ListType => ImportListType.Advanced;

        public CustomBookImport(ICustomBookImportProxy customProxy,
                            IImportListStatusService importListStatusService,
                            IConfigService configService,
                            IParsingService parsingService,
                            ILocalizationService localizationService,
                            Logger logger)
            : base(importListStatusService, configService, parsingService, localizationService, logger)
        {
            _customProxy = customProxy;
        }

        public override ImportListBookFetchResult Fetch()
        {
            var books = new List<ImportListBookInfo>();
            var anyFailure = false;

            try
            {
                var remoteBooks = _customProxy.GetBooks(Settings);

                foreach (var item in remoteBooks)
                {
                    books.Add(new ImportListBookInfo
                    {
                        Title = item.Title.IsNullOrWhiteSpace() ? $"ISBN: {item.Isbn}" : item.Title,
                        AuthorName = item.AuthorName,
                        Isbn = item.Isbn,
                        Asin = item.Asin,
                        GoodreadsId = item.GoodreadsId,
                        Year = item.Year,
                        Publisher = item.Publisher,
                        ReleaseDate = item.ReleaseDate
                    });
                }

                _importListStatusService.RecordSuccess(Definition.Id);
            }
            catch (Exception ex)
            {
                anyFailure = true;
                _logger.Debug(ex, "Failed to fetch data for list {0} ({1})", Definition.Name, Name);

                _importListStatusService.RecordFailure(Definition.Id);
            }

            return new ImportListBookFetchResult(CleanupListItems(books), anyFailure);
        }

        public override object RequestAction(string action, IDictionary<string, string> query)
        {
            return new { };
        }

        protected override void Test(List<ValidationFailure> failures)
        {
            failures.AddIfNotNull(_customProxy.Test(Settings));
        }
    }
}