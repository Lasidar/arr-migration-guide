using System.Collections.Generic;
using NzbDrone.Core.MediaFiles;
using NzbDrone.Core.ThingiProvider;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.Notifications
{
    public interface INotification : IProvider
    {
        string Link { get; }

        void OnGrab(GrabMessage grabMessage);
        void OnDownload(DownloadMessage message);
        void OnRename(Series series, List<RenamedEditionFile> renamedFiles);
        void OnImportComplete(ImportCompleteMessage message);
        void OnEditionFileDelete(EpisodeDeleteMessage deleteMessage);
        void OnSeriesAdd(SeriesAddMessage message);
        void OnSeriesDelete(SeriesDeleteMessage deleteMessage);
        void OnHealthIssue(HealthCheck.HealthCheck healthCheck);
        void OnHealthRestored(HealthCheck.HealthCheck previousCheck);
        void OnApplicationUpdate(ApplicationUpdateMessage updateMessage);
        void OnManualInteractionRequired(ManualInteractionRequiredMessage message);
        void ProcessQueue();
        bool SupportsOnGrab { get; }
        bool SupportsOnDownload { get; }
        bool SupportsOnUpgrade { get; }
        bool SupportsOnImportComplete { get; }
        bool SupportsOnRename { get; }
        bool SupportsOnSeriesAdd { get; }
        bool SupportsOnSeriesDelete { get; }
        bool SupportsOnEditionFileDelete { get; }
        bool SupportsOnEditionFileDeleteForUpgrade { get; }
        bool SupportsOnHealthIssue { get; }
        bool SupportsOnHealthRestored { get; }
        bool SupportsOnApplicationUpdate { get; }
        bool SupportsOnManualInteractionRequired { get; }
    }
}
