using System;
using NLog;
using Readarr.Common.Http;
using Readarr.Core.Configuration;
using Readarr.Core.Localization;
using Readarr.Core.Parser;

namespace Readarr.Core.ImportLists.Rss
{
    public class RssBookImportBase<TSettings> : HttpBookImportListBase<TSettings>
        where TSettings : RssImportBaseSettings<TSettings>, new()
    {
        public override string Name => "RSS Book List";
        public override ImportListType ListType => ImportListType.Advanced;
        public override TimeSpan MinRefreshInterval => TimeSpan.FromHours(6);

        public RssBookImportBase(IHttpClient httpClient,
            IImportListStatusService importListStatusService,
            IConfigService configService,
            IParsingService parsingService,
            ILocalizationService localizationService,
            Logger logger)
            : base(httpClient, importListStatusService, configService, parsingService, localizationService, logger)
        {
        }

        public override ImportListBookFetchResult Fetch()
        {
            return FetchItems(g => g.GetListItems());
        }

        public override IParseImportListResponse GetParser()
        {
            return new RssBookImportParser(_logger);
        }

        public override IImportListRequestGenerator GetRequestGenerator()
        {
            return new RssImportRequestGenerator<TSettings>
            {
                Settings = Settings
            };
        }
    }
}