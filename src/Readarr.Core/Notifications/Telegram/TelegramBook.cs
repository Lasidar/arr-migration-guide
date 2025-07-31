using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using Readarr.Common.Extensions;
using Readarr.Core.Books;
using Readarr.Core.Configuration;
using Readarr.Core.Localization;
using Readarr.Core.MediaFiles;

namespace Readarr.Core.Notifications.Telegram
{
    public class TelegramBook : BookNotificationBase<TelegramSettings>
    {
        private readonly ITelegramProxy _proxy;
        private readonly IConfigFileProvider _configFileProvider;
        private readonly ILocalizationService _localizationService;

        public TelegramBook(ITelegramProxy proxy, IConfigFileProvider configFileProvider, ILocalizationService localizationService)
        {
            _proxy = proxy;
            _configFileProvider = configFileProvider;
            _localizationService = localizationService;
        }

        public override string Name => "Telegram";
        public override string Link => "https://telegram.org/";

        private string InstanceName => _configFileProvider.InstanceName;

        public override void OnGrab(GrabMessage message)
        {
            var author = message.Book.Author;
            var books = message.Book.Books;
            var quality = message.Quality;

            var title = "Book Grabbed";
            if (Settings.IncludeAppNameInTitle)
            {
                title = $"Readarr - {title}";
            }
            if (Settings.IncludeInstanceNameInTitle)
            {
                title = $"{title} - {InstanceName}";
            }

            var body = $"{author.Name} - {string.Join(", ", books.Select(b => b.Title))} [{quality.Quality.Name}]";
            var links = GetLinks(author);

            _proxy.SendNotification(title, body, links, Settings);
        }

        public override void OnImportComplete(ImportCompleteMessage message)
        {
            var author = message.Author;
            var book = message.BookFile.Books.Value.First();
            
            var title = message.IsUpgrade ? "Book Upgraded" : "Book Downloaded";
            if (Settings.IncludeAppNameInTitle)
            {
                title = $"Readarr - {title}";
            }
            if (Settings.IncludeInstanceNameInTitle)
            {
                title = $"{title} - {InstanceName}";
            }

            var body = $"{author.Name} - {book.Title}";
            var links = GetLinks(author);

            _proxy.SendNotification(title, body, links, Settings);
        }

        public override void OnRename(Author author, List<RenamedBookFile> renamedFiles)
        {
            var title = "Books Renamed";
            if (Settings.IncludeAppNameInTitle)
            {
                title = $"Readarr - {title}";
            }
            if (Settings.IncludeInstanceNameInTitle)
            {
                title = $"{title} - {InstanceName}";
            }

            var body = $"{renamedFiles.Count} book files renamed for {author.Name}";
            var links = GetLinks(author);

            _proxy.SendNotification(title, body, links, Settings);
        }

        public override void OnAuthorAdd(AuthorAddMessage message)
        {
            var title = "Author Added";
            if (Settings.IncludeAppNameInTitle)
            {
                title = $"Readarr - {title}";
            }
            if (Settings.IncludeInstanceNameInTitle)
            {
                title = $"{title} - {InstanceName}";
            }

            var body = $"{message.Author.Name}";
            var links = GetLinks(message.Author);

            _proxy.SendNotification(title, body, links, Settings);
        }

        public override void OnAuthorDelete(AuthorDeleteMessage message)
        {
            var title = "Author Deleted";
            if (Settings.IncludeAppNameInTitle)
            {
                title = $"Readarr - {title}";
            }
            if (Settings.IncludeInstanceNameInTitle)
            {
                title = $"{title} - {InstanceName}";
            }

            var body = $"{message.Author.Name}";

            _proxy.SendNotification(title, body, new List<string>(), Settings);
        }

        public override void OnBookFileDelete(BookFileDeleteMessage message)
        {
            var title = "Book File Deleted";
            if (Settings.IncludeAppNameInTitle)
            {
                title = $"Readarr - {title}";
            }
            if (Settings.IncludeInstanceNameInTitle)
            {
                title = $"{title} - {InstanceName}";
            }

            var body = $"{message.Author.Name} - {message.Book.Title}";
            var links = GetLinks(message.Author);

            _proxy.SendNotification(title, body, links, Settings);
        }

        public override void OnHealthIssue(HealthCheck.HealthCheck healthCheck)
        {
            var title = "Health Issue";
            if (Settings.IncludeAppNameInTitle)
            {
                title = $"Readarr - {title}";
            }
            if (Settings.IncludeInstanceNameInTitle)
            {
                title = $"{title} - {InstanceName}";
            }

            _proxy.SendNotification(title, healthCheck.Message, new List<string>(), Settings);
        }

        public override void OnHealthRestored(HealthCheck.HealthCheck previousCheck)
        {
            var title = "Health Issue Resolved";
            if (Settings.IncludeAppNameInTitle)
            {
                title = $"Readarr - {title}";
            }
            if (Settings.IncludeInstanceNameInTitle)
            {
                title = $"{title} - {InstanceName}";
            }

            var body = $"The following issue has been resolved: {previousCheck.Message}";

            _proxy.SendNotification(title, body, new List<string>(), Settings);
        }

        public override void OnApplicationUpdate(ApplicationUpdateMessage updateMessage)
        {
            var title = "Application Updated";
            if (Settings.IncludeAppNameInTitle)
            {
                title = $"Readarr - {title}";
            }
            if (Settings.IncludeInstanceNameInTitle)
            {
                title = $"{title} - {InstanceName}";
            }

            var body = $"Readarr has been updated to version {updateMessage.NewVersion}";

            _proxy.SendNotification(title, body, new List<string>(), Settings);
        }

        public override void OnManualInteractionRequired(ManualInteractionRequiredMessage message)
        {
            var author = message.Author;
            var books = message.Book.Books;
            
            var title = "Manual Interaction Required";
            if (Settings.IncludeAppNameInTitle)
            {
                title = $"Readarr - {title}";
            }
            if (Settings.IncludeInstanceNameInTitle)
            {
                title = $"{title} - {InstanceName}";
            }

            var body = $"{author.Name} - {string.Join(", ", books.Select(b => b.Title))}";
            var links = GetLinks(author);

            _proxy.SendNotification(title, body, links, Settings);
        }

        public override ValidationResult Test()
        {
            var failures = new List<ValidationFailure>();

            failures.AddIfNotNull(_proxy.Test(Settings));

            return new ValidationResult(failures);
        }

        private List<string> GetLinks(Author author)
        {
            var links = new List<string>();

            if (Settings.SendSilently)
            {
                links.Add("silent");
            }

            if (author.Metadata.Value.GoodreadsId.IsNotNullOrWhiteSpace())
            {
                links.Add($"[View on Goodreads](https://www.goodreads.com/author/show/{author.Metadata.Value.GoodreadsId})");
            }

            return links;
        }
    }
}