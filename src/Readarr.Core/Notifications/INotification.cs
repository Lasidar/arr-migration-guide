using System.Collections.Generic;
using Readarr.Core.MediaFiles;
using Readarr.Core.ThingiProvider;
using Readarr.Core.Books;

namespace Readarr.Core.Notifications
{
    public interface INotification : IProvider
    {
        string Link { get; }

        void OnGrab(GrabMessage grabMessage);
        void OnDownload(DownloadMessage message);
        void OnRename(Series series, List<RenamedBookFile> renamedFiles);
        void OnImportComplete(ImportCompleteMessage message);
        void OnEpisodeFileDelete(EpisodeDeleteMessage deleteMessage);
        void OnSeriesAdd(AuthorAddMessage message);
        void OnSeriesDelete(AuthorDeleteMessage deleteMessage);
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
        bool SupportsOnEpisodeFileDelete { get; }
        bool SupportsOnEpisodeFileDeleteForUpgrade { get; }
        bool SupportsOnHealthIssue { get; }
        bool SupportsOnHealthRestored { get; }
        bool SupportsOnApplicationUpdate { get; }
        bool SupportsOnManualInteractionRequired { get; }
    }
}
