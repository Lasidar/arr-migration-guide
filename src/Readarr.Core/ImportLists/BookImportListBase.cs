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
using Readarr.Core.ThingiProvider;

namespace Readarr.Core.ImportLists
{
    public abstract class BookImportListBase<TSettings> : IBookImportList
        where TSettings : IImportListSettings, new()
    {
        protected readonly IImportListStatusService _importListStatusService;
        protected readonly IConfigService _configService;
        protected readonly IParsingService _parsingService;
        protected readonly ILocalizationService _localizationService;
        protected readonly Logger _logger;

        public abstract string Name { get; }

        public abstract ImportListType ListType { get; }

        public abstract TimeSpan MinRefreshInterval { get; }

        public BookImportListBase(IImportListStatusService importListStatusService, IConfigService configService, IParsingService parsingService, ILocalizationService localizationService, Logger logger)
        {
            _importListStatusService = importListStatusService;
            _configService = configService;
            _parsingService = parsingService;
            _localizationService = localizationService;
            _logger = logger;
        }

        public Type ConfigContract => typeof(TSettings);

        public virtual ProviderMessage Message => null;

        public virtual IEnumerable<ProviderDefinition> DefaultDefinitions
        {
            get
            {
                var config = (IProviderConfig)new TSettings();

                yield return new ImportListDefinition
                {
                    Name = GetType().Name,
                    EnableAutomaticAdd = config.EnableAutomaticAdd,
                    Implementation = GetType().Name,
                    Settings = config
                };
            }
        }

        public virtual ProviderDefinition Definition { get; set; }

        public virtual object RequestAction(string action, IDictionary<string, string> query) { return null; }

        protected TSettings Settings => (TSettings)Definition.Settings;

        public abstract ImportListBookFetchResult Fetch();

        protected virtual IList<ImportListBookInfo> CleanupListItems(IEnumerable<ImportListBookInfo> releases)
        {
            var result = releases.DistinctBy(r => new { r.Title, r.AuthorName, r.Isbn, r.Asin, r.GoodreadsId }).ToList();

            result.ForEach(c =>
            {
                c.ImportListId = Definition.Id;
                c.ImportList = Definition.Name;
            });

            return result;
        }

        public ValidationResult Test()
        {
            var failures = new List<ValidationFailure>();

            try
            {
                Test(failures);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Test aborted due to exception");
                failures.Add(new ValidationFailure(string.Empty, _localizationService.GetLocalizedString("ImportListTestException", new Dictionary<string, object> { { "exceptionMessage", ex.Message } })));
            }

            return new ValidationResult(failures);
        }

        protected abstract void Test(List<ValidationFailure> failures);

        public override string ToString()
        {
            return Definition.Name;
        }
    }
}