using System.Collections.Generic;
using Readarr.Common.Disk;
using Readarr.Common.Extensions;
using Readarr.Core.Configuration;
using Readarr.Core.Localization;
using Readarr.Core.MediaFiles.Events;

namespace Readarr.Core.HealthCheck.Checks
{
    [CheckOn(typeof(BooksImportedEvent), CheckOnCondition.FailedOnly)]
    [CheckOn(typeof(EpisodeImportFailedEvent), CheckOnCondition.SuccessfulOnly)]
    public class RecyclingBinCheck : HealthCheckBase
    {
        private readonly IConfigService _configService;
        private readonly IDiskProvider _diskProvider;

        public RecyclingBinCheck(IConfigService configService, IDiskProvider diskProvider, ILocalizationService localizationService)
            : base(localizationService)
        {
            _configService = configService;
            _diskProvider = diskProvider;
        }

        public override HealthCheck Check()
        {
            var recycleBin = _configService.RecycleBin;

            if (recycleBin.IsNullOrWhiteSpace())
            {
                return new HealthCheck(GetType());
            }

            if (!_diskProvider.FolderWritable(recycleBin))
            {
                return new HealthCheck(GetType(),
                    HealthCheckResult.Error,
                    _localizationService.GetLocalizedString("RecycleBinUnableToWriteHealthCheckMessage", new Dictionary<string, object>
                    {
                        { "path", recycleBin }
                    }),
                    "#cannot-write-recycle-bin");
            }

            return new HealthCheck(GetType());
        }
    }
}
