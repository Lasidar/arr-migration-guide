using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using Readarr.Common.Extensions;
using Readarr.Core.Books;
using Readarr.Core.MediaFiles;

namespace Readarr.Core.Notifications.Pushover
{
    public class PushoverBook : BookNotificationBase<PushoverSettings>
    {
        private readonly IPushoverProxy _proxy;

        public PushoverBook(IPushoverProxy proxy)
        {
            _proxy = proxy;
        }

        public override string Name => "Pushover";
        public override string Link => "https://pushover.net/";

        public override void OnGrab(GrabMessage grabMessage)
        {
            var author = grabMessage.Book.Author;
            var books = grabMessage.Book.Books;
            var quality = grabMessage.Quality;

            var title = Settings.IncludeAuthorName ? $"{author.Name} - Book Grabbed" : "Book Grabbed";
            var message = $"{string.Join(", ", books.Select(b => b.Title))} [{quality.Quality.Name}]";

            _proxy.SendNotification(title, message, Settings);
        }

        public override void OnImportComplete(ImportCompleteMessage message)
        {
            var author = message.Author;
            var book = message.BookFile.Books.Value.First();
            
            var title = Settings.IncludeAuthorName ? 
                $"{author.Name} - {(message.IsUpgrade ? "Book Upgraded" : "Book Downloaded")}" : 
                (message.IsUpgrade ? "Book Upgraded" : "Book Downloaded");
            
            var body = book.Title;

            _proxy.SendNotification(title, body, Settings);
        }

        public override void OnRename(Author author, List<RenamedBookFile> renamedFiles)
        {
            var title = Settings.IncludeAuthorName ? $"{author.Name} - Books Renamed" : "Books Renamed";
            var message = $"{renamedFiles.Count} book files renamed";

            _proxy.SendNotification(title, message, Settings);
        }

        public override void OnAuthorAdd(AuthorAddMessage message)
        {
            var title = "Author Added";
            var body = message.Author.Name;

            _proxy.SendNotification(title, body, Settings);
        }

        public override void OnAuthorDelete(AuthorDeleteMessage deleteMessage)
        {
            var title = "Author Deleted";
            var body = deleteMessage.Author.Name;

            _proxy.SendNotification(title, body, Settings);
        }

        public override void OnBookFileDelete(BookFileDeleteMessage deleteMessage)
        {
            var author = deleteMessage.Author;
            var book = deleteMessage.Book;
            
            var title = Settings.IncludeAuthorName ? $"{author.Name} - Book File Deleted" : "Book File Deleted";
            var body = book.Title;

            _proxy.SendNotification(title, body, Settings);
        }

        public override void OnHealthIssue(HealthCheck.HealthCheck healthCheck)
        {
            var title = Settings.IncludeHealthWarnings ? "Health Issue" : null;
            if (title.IsNullOrWhiteSpace()) return;

            _proxy.SendNotification(title, healthCheck.Message, Settings);
        }

        public override void OnHealthRestored(HealthCheck.HealthCheck previousCheck)
        {
            var title = Settings.IncludeHealthWarnings ? "Health Issue Resolved" : null;
            if (title.IsNullOrWhiteSpace()) return;

            _proxy.SendNotification(title, $"The following issue has been resolved: {previousCheck.Message}", Settings);
        }

        public override void OnApplicationUpdate(ApplicationUpdateMessage updateMessage)
        {
            var title = "Readarr Updated";
            var body = $"Readarr has been updated to version {updateMessage.NewVersion}";

            _proxy.SendNotification(title, body, Settings);
        }

        public override void OnManualInteractionRequired(ManualInteractionRequiredMessage message)
        {
            var author = message.Author;
            var books = message.Book.Books;
            
            var title = Settings.IncludeAuthorName ? $"{author.Name} - Manual Interaction Required" : "Manual Interaction Required";
            var body = $"{string.Join(", ", books.Select(b => b.Title))}";

            _proxy.SendNotification(title, body, Settings);
        }

        public override ValidationResult Test()
        {
            var failures = new List<ValidationFailure>();

            failures.AddIfNotNull(_proxy.Test(Settings));

            return new ValidationResult(failures);
        }
    }
}