using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using Readarr.Common.Extensions;
using Readarr.Core.Books;
using Readarr.Core.Localization;
using Readarr.Core.MediaFiles;

namespace Readarr.Core.Notifications.Pushover
{
    public class PushoverBook : BookNotificationBase<PushoverSettings>
    {
        private readonly IPushoverProxy _proxy;
        private readonly ILocalizationService _localizationService;

        public PushoverBook(IPushoverProxy proxy, ILocalizationService localizationService)
        {
            _proxy = proxy;
            _localizationService = localizationService;
        }

        public override string Name => "Pushover";
        public override string Link => "https://pushover.net/";

        public override void OnGrab(GrabMessage message)
        {
            var author = message.Book.Author;
            var books = message.Book.Books;
            var quality = message.Quality;

            var title = "Book Grabbed";
            var body = $"{author.Name} - {string.Join(", ", books.Select(b => b.Title))} [{quality.Quality.Name}]";

            _proxy.SendNotification(title, body, Settings);
        }

        public override void OnImportComplete(ImportCompleteMessage message)
        {
            var author = message.Author;
            var book = message.BookFile.Books.Value.First();
            
            var title = message.IsUpgrade ? "Book Upgraded" : "Book Downloaded";
            var body = $"{author.Name} - {book.Title}";

            _proxy.SendNotification(title, body, Settings);
        }

        public override void OnRename(Author author, List<RenamedBookFile> renamedFiles)
        {
            var title = "Books Renamed";
            var body = $"{renamedFiles.Count} book files renamed for {author.Name}";

            _proxy.SendNotification(title, body, Settings);
        }

        public override void OnAuthorAdd(AuthorAddMessage message)
        {
            var title = "Author Added";
            var body = $"{message.Author.Name}";

            _proxy.SendNotification(title, body, Settings);
        }

        public override void OnAuthorDelete(AuthorDeleteMessage message)
        {
            var title = "Author Deleted";
            var body = $"{message.Author.Name}";

            _proxy.SendNotification(title, body, Settings);
        }

        public override void OnBookFileDelete(BookFileDeleteMessage message)
        {
            var title = "Book File Deleted";
            var body = $"{message.Author.Name} - {message.Book.Title}";

            _proxy.SendNotification(title, body, Settings);
        }

        public override void OnHealthIssue(HealthCheck.HealthCheck healthCheck)
        {
            var title = "Health Issue";
            var body = healthCheck.Message;

            _proxy.SendNotification(title, body, Settings, (int)healthCheck.Type);
        }

        public override void OnHealthRestored(HealthCheck.HealthCheck previousCheck)
        {
            var title = "Health Issue Resolved";
            var body = $"The following issue has been resolved: {previousCheck.Message}";

            _proxy.SendNotification(title, body, Settings);
        }

        public override void OnApplicationUpdate(ApplicationUpdateMessage updateMessage)
        {
            var title = "Application Updated";
            var body = $"Readarr updated to {updateMessage.NewVersion}";

            _proxy.SendNotification(title, body, Settings);
        }

        public override void OnManualInteractionRequired(ManualInteractionRequiredMessage message)
        {
            var author = message.Author;
            var books = message.Book.Books;
            
            var title = "Manual Interaction Required";
            var body = $"{author.Name} - {string.Join(", ", books.Select(b => b.Title))}";

            _proxy.SendNotification(title, body, Settings, (int)PushoverPriority.High);
        }

        public override ValidationResult Test()
        {
            var failures = new List<ValidationFailure>();

            failures.AddIfNotNull(_proxy.Test(Settings));

            return new ValidationResult(failures);
        }
    }
}