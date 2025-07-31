using System;
using System.Collections.Generic;
using System.IO;
using FluentValidation.Results;
using NLog;
using Readarr.Common.Disk;
using Readarr.Common.Extensions;
using Readarr.Common.Http;
using Readarr.Core.Configuration;
using Readarr.Core.Localization;
using Readarr.Core.Organizer;
using Readarr.Core.Parser.Model;
using Readarr.Core.RemotePathMappings;
using Readarr.Core.Tv;

namespace Readarr.Core.Download.Clients.Blackhole
{
    public class UsenetBlackhole : UsenetClientBase<UsenetBlackholeSettings>
    {
        private readonly IScanWatchFolder _scanWatchFolder;

        public TimeSpan ScanGracePeriod { get; set; }

        public UsenetBlackhole(IScanWatchFolder scanWatchFolder,
                               IHttpClient httpClient,
                               IConfigService configService,
                               IDiskProvider diskProvider,
                               IRemotePathMappingService remotePathMappingService,
                               IValidateNzbs nzbValidationService,
                               Logger logger,
                               ILocalizationService localizationService)
            : base(httpClient, configService, diskProvider, remotePathMappingService, nzbValidationService, logger, localizationService)
        {
            _scanWatchFolder = scanWatchFolder;

            ScanGracePeriod = TimeSpan.FromSeconds(30);
        }

        protected override string AddFromNzbFile(RemoteEpisode remoteEpisode, string filename, byte[] fileContent)
        {
            var title = remoteEpisode.Release.Title;

            title = FileNameBuilder.CleanFileName(title);

            var filepath = Path.Combine(Settings.NzbFolder, title + ".nzb");

            using (var stream = _diskProvider.OpenWriteStream(filepath))
            {
                stream.Write(fileContent, 0, fileContent.Length);
            }

            _logger.Debug("NZB Download succeeded, saved to: {0}", filepath);

            return null;
        }

        public override string Name => _localizationService.GetLocalizedString("UsenetBlackhole");

        public override IEnumerable<DownloadClientItem> GetItems()
        {
            foreach (var item in _scanWatchFolder.GetItems(Settings.WatchFolder, ScanGracePeriod))
            {
                var queueItem = new DownloadClientItem
                {
                    DownloadClientInfo = DownloadClientItemClientInfo.FromDownloadClient(this),
                    DownloadId = Definition.Name + "_" + item.DownloadId,
                    Title = item.Title,

                    TotalSize = item.TotalSize,

                    OutputPath = item.OutputPath,

                    Status = item.Status,
                };

                queueItem.CanMoveFiles = true;
                queueItem.CanBeRemoved = queueItem.DownloadClientInfo.RemoveCompletedDownloads;

                yield return queueItem;
            }
        }

        public override void RemoveItem(DownloadClientItem item, bool deleteData)
        {
            if (!deleteData)
            {
                throw new NotSupportedException("Blackhole cannot remove DownloadItem without deleting the data as well, ignoring.");
            }

            DeleteItemData(item);
        }

        public override DownloadClientInfo GetStatus()
        {
            return new DownloadClientInfo
            {
                IsLocalhost = true,
                OutputRootFolders = new List<OsPath> { new OsPath(Settings.WatchFolder) }
            };
        }

        protected override void Test(List<ValidationFailure> failures)
        {
            failures.AddIfNotNull(TestFolder(Settings.NzbFolder, "NzbFolder"));
            failures.AddIfNotNull(TestFolder(Settings.WatchFolder, "WatchFolder"));
        }
    }
}
