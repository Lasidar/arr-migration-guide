using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using Readarr.Common.Extensions;
using Readarr.Core.Books;
using Readarr.Core.Localization;
using Readarr.Core.MediaFiles;

namespace Readarr.Core.Notifications.Telegram
{
    public class TelegramBook : BookNotificationBase<TelegramSettings>
    {
        private readonly ITelegramProxy _proxy;
        private readonly ILocalizationService _localizationService;

        public TelegramBook(ITelegramProxy proxy, ILocalizationService localizationService)
        {
            _proxy = proxy;
            _localizationService = localizationService;
        }

        public override string Name => "Telegram";
        public override string Link => "https://telegram.org/";

        public override void OnGrab(GrabMessage grabMessage)
        {
            var author = grabMessage.Book.Author;
            var books = grabMessage.Book.Books;
            var quality = grabMessage.Quality;

            var title = "📚 Book Grabbed";
            var message = $"<b>{author.Name}</b>\n{string.Join(", ", books.Select(b => b.Title))}\n<i>Quality: {quality.Quality.Name}</i>";

            _proxy.SendNotification(title, message, Settings);
        }

        public override void OnImportComplete(ImportCompleteMessage message)
        {
            var author = message.Author;
            var book = message.BookFile.Books.Value.First();
            
            var title = message.IsUpgrade ? "📖 Book Upgraded" : "📖 Book Downloaded";
            var body = $"<b>{author.Name}</b>\n{book.Title}";

            _proxy.SendNotification(title, body, Settings);
        }

        public override void OnRename(Author author, List<RenamedBookFile> renamedFiles)
        {
            var title = "📝 Books Renamed";
            var message = $"<b>{author.Name}</b>\n{renamedFiles.Count} book files renamed";

            _proxy.SendNotification(title, message, Settings);
        }

        public override void OnAuthorAdd(AuthorAddMessage message)
        {
            var title = "✍️ Author Added";
            var body = $"<b>{message.Author.Name}</b>\n{message.Author.Metadata.Value?.Overview?.Truncate(200)}";

            _proxy.SendNotification(title, body, Settings);
        }

        public override void OnAuthorDelete(AuthorDeleteMessage deleteMessage)
        {
            var title = "🗑️ Author Deleted";
            var body = $"<b>{deleteMessage.Author.Name}</b>";

            _proxy.SendNotification(title, body, Settings);
        }

        public override void OnBookFileDelete(BookFileDeleteMessage deleteMessage)
        {
            var title = "🗑️ Book File Deleted";
            var body = $"<b>{deleteMessage.Author.Name}</b>\n{deleteMessage.Book.Title}\n<i>Reason: {deleteMessage.Reason}</i>";

            _proxy.SendNotification(title, body, Settings);
        }

        public override void OnHealthIssue(HealthCheck.HealthCheck healthCheck)
        {
            var title = healthCheck.Type == HealthCheck.HealthCheckResult.Warning ? "⚠️ Health Warning" : "❌ Health Error";
            var body = healthCheck.Message;

            _proxy.SendNotification(title, body, Settings);
        }

        public override void OnHealthRestored(HealthCheck.HealthCheck previousCheck)
        {
            var title = "✅ Health Restored";
            var body = $"The following issue has been resolved:\n{previousCheck.Message}";

            _proxy.SendNotification(title, body, Settings);
        }

        public override void OnApplicationUpdate(ApplicationUpdateMessage updateMessage)
        {
            var title = "🔄 Readarr Updated";
            var body = $"Readarr has been updated to version <b>{updateMessage.NewVersion}</b>";

            _proxy.SendNotification(title, body, Settings);
        }

        public override void OnManualInteractionRequired(ManualInteractionRequiredMessage message)
        {
            var author = message.Author;
            var books = message.Book.Books;
            
            var title = "⚠️ Manual Interaction Required";
            var body = $"<b>{author.Name}</b>\n{string.Join(", ", books.Select(b => b.Title))}\n<i>Download requires manual interaction</i>";

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