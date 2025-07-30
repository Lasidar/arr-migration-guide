using System.Collections.Generic;
using Readarr.Core.MediaFiles;
using Readarr.Core.ThingiProvider;
using Readarr.Core.Books;

namespace Readarr.Core.Notifications
{
    public interface IBookNotification : IProvider
    {
        string Link { get; }

        void OnGrab(GrabMessage grabMessage);
        void OnDownload(DownloadMessage message);
        void OnRename(Author author, List<RenamedBookFile> renamedFiles);
        void OnImportComplete(ImportCompleteMessage message);
        void OnBookFileDelete(BookFileDeleteMessage deleteMessage);
        void OnAuthorAdd(AuthorAddMessage message);
        void OnAuthorDelete(AuthorDeleteMessage deleteMessage);
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
        bool SupportsOnAuthorAdd { get; }
        bool SupportsOnAuthorDelete { get; }
        bool SupportsOnBookFileDelete { get; }
        bool SupportsOnBookFileDeleteForUpgrade { get; }
        bool SupportsOnHealthIssue { get; }
        bool SupportsOnHealthRestored { get; }
        bool SupportsOnApplicationUpdate { get; }
        bool SupportsOnManualInteractionRequired { get; }
    }
}