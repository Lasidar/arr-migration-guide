using System;
using System.Collections.Generic;
using FluentValidation.Results;
using Readarr.Core.Books;
using Readarr.Core.MediaFiles;
using Readarr.Core.ThingiProvider;

namespace Readarr.Core.Notifications
{
    public abstract class BookNotificationBase<TSettings> : IBookNotification
        where TSettings : NotificationSettingsBase<TSettings>, new()
    {
        protected const string BOOK_GRABBED_TITLE = "Book Grabbed";
        protected const string BOOK_DOWNLOADED_TITLE = "Book Downloaded";
        protected const string IMPORT_COMPLETE_TITLE = "Import Complete";
        protected const string BOOK_DELETED_TITLE = "Book Deleted";
        protected const string AUTHOR_ADDED_TITLE = "Author Added";
        protected const string AUTHOR_DELETED_TITLE = "Author Deleted";
        protected const string HEALTH_ISSUE_TITLE = "Health Check Failure";
        protected const string HEALTH_RESTORED_TITLE = "Health Check Restored";
        protected const string APPLICATION_UPDATE_TITLE = "Application Updated";
        protected const string MANUAL_INTERACTION_REQUIRED_TITLE = "Manual Interaction";

        protected const string BOOK_GRABBED_TITLE_BRANDED = "Readarr - " + BOOK_GRABBED_TITLE;
        protected const string BOOK_DOWNLOADED_TITLE_BRANDED = "Readarr - " + BOOK_DOWNLOADED_TITLE;
        protected const string IMPORT_COMPLETE_TITLE_BRANDED = "Readarr - " + IMPORT_COMPLETE_TITLE;
        protected const string BOOK_DELETED_TITLE_BRANDED = "Readarr - " + BOOK_DELETED_TITLE;
        protected const string AUTHOR_ADDED_TITLE_BRANDED = "Readarr - " + AUTHOR_ADDED_TITLE;
        protected const string AUTHOR_DELETED_TITLE_BRANDED = "Readarr - " + AUTHOR_DELETED_TITLE;
        protected const string HEALTH_ISSUE_TITLE_BRANDED = "Readarr - " + HEALTH_ISSUE_TITLE;
        protected const string HEALTH_RESTORED_TITLE_BRANDED = "Readarr - " + HEALTH_RESTORED_TITLE;
        protected const string APPLICATION_UPDATE_TITLE_BRANDED = "Readarr - " + APPLICATION_UPDATE_TITLE;
        protected const string MANUAL_INTERACTION_REQUIRED_TITLE_BRANDED = "Readarr - " + MANUAL_INTERACTION_REQUIRED_TITLE;

        public abstract string Name { get; }

        public Type ConfigContract => typeof(TSettings);

        public virtual ProviderMessage Message => null;

        public IEnumerable<ProviderDefinition> DefaultDefinitions => new List<ProviderDefinition>();

        public ProviderDefinition Definition { get; set; }
        public abstract ValidationResult Test();

        public abstract string Link { get; }

        public virtual void OnGrab(GrabMessage grabMessage)
        {
        }

        public virtual void OnDownload(DownloadMessage message)
        {
        }

        public virtual void OnImportComplete(ImportCompleteMessage message)
        {
        }

        public virtual void OnRename(Author author, List<RenamedBookFile> renamedFiles)
        {
        }

        public virtual void OnBookFileDelete(BookFileDeleteMessage deleteMessage)
        {
        }

        public virtual void OnAuthorAdd(AuthorAddMessage message)
        {
        }

        public virtual void OnAuthorDelete(AuthorDeleteMessage deleteMessage)
        {
        }

        public virtual void OnHealthIssue(HealthCheck.HealthCheck healthCheck)
        {
        }

        public virtual void OnHealthRestored(HealthCheck.HealthCheck previousCheck)
        {
        }

        public virtual void OnApplicationUpdate(ApplicationUpdateMessage updateMessage)
        {
        }

        public virtual void OnManualInteractionRequired(ManualInteractionRequiredMessage message)
        {
        }

        public virtual void ProcessQueue()
        {
        }

        public bool SupportsOnGrab => HasConcreteImplementation("OnGrab");
        public bool SupportsOnRename => HasConcreteImplementation("OnRename");
        public bool SupportsOnDownload => HasConcreteImplementation("OnDownload");
        public bool SupportsOnUpgrade => SupportsOnDownload;
        public bool SupportsOnImportComplete => HasConcreteImplementation("OnImportComplete");
        public bool SupportsOnAuthorAdd => HasConcreteImplementation("OnAuthorAdd");
        public bool SupportsOnAuthorDelete => HasConcreteImplementation("OnAuthorDelete");
        public bool SupportsOnBookFileDelete => HasConcreteImplementation("OnBookFileDelete");
        public bool SupportsOnBookFileDeleteForUpgrade => SupportsOnBookFileDelete;
        public bool SupportsOnHealthIssue => HasConcreteImplementation("OnHealthIssue");
        public bool SupportsOnHealthRestored => HasConcreteImplementation("OnHealthRestored");
        public bool SupportsOnApplicationUpdate => HasConcreteImplementation("OnApplicationUpdate");
        public bool SupportsOnManualInteractionRequired => HasConcreteImplementation("OnManualInteractionRequired");

        protected TSettings Settings => (TSettings)Definition.Settings;

        public override string ToString()
        {
            return GetType().Name;
        }

        public virtual object RequestAction(string action, IDictionary<string, string> query)
        {
            return null;
        }

        private bool HasConcreteImplementation(string methodName)
        {
            var method = GetType().GetMethod(methodName);

            if (method == null)
            {
                throw new MissingMethodException(GetType().Name, methodName);
            }

            return !method.DeclaringType.IsAbstract;
        }
    }
}