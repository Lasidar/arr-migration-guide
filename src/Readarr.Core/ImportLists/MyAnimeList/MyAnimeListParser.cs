using System.Collections.Generic;
using NLog;
using Readarr.Common.Extensions;
using Readarr.Common.Instrumentation;
using Readarr.Common.Serializer;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.ImportLists.MyAnimeList
{
    public class MyAnimeListParser : IParseImportListResponse
    {
        private static readonly Logger Logger = ReadarrLogger.GetLogger(typeof(MyAnimeListParser));

        public IList<ImportListItemInfo> ParseResponse(ImportListResponse importListResponse)
        {
            var jsonResponse = Json.Deserialize<MyAnimeListResponse>(importListResponse.Content);
            var series = new List<ImportListItemInfo>();

            foreach (var show in jsonResponse.Animes)
            {
                series.AddIfNotNull(new ImportListItemInfo
                {
                    Title = show.AnimeListInfo.Title,
                    MalId = show.AnimeListInfo.Id
                });
            }

            return series;
        }
    }
}
