using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentValidation.Results;
using NLog;
using Readarr.Common.Extensions;
using Readarr.Core.Books;
using Readarr.Core.Localization;
using Readarr.Core.MediaCover;
using Readarr.Core.MediaFiles;

namespace Readarr.Core.Notifications.Gotify
{
    public class GotifyBook : BookNotificationBase<GotifySettings>
    {
        private const string ReadarrImageUrl = "https://raw.githubusercontent.com/Readarr/Readarr/develop/Logo/128.png";

        private readonly IGotifyProxy _proxy;
        private readonly ILocalizationService _localizationService;
        private readonly Logger _logger;

        public GotifyBook(IGotifyProxy proxy, ILocalizationService localizationService, Logger logger)
        {
            _proxy = proxy;
            _localizationService = localizationService;
            _logger = logger;
        }

        public override string Name => "Gotify";
        public override string Link => "https://gotify.net/";

        public override void OnGrab(GrabMessage message)
        {
            var author = message.Book.Author;
            var books = message.Book.Books;
            var quality = message.Quality;

            var title = "Book Grabbed";
            var body = $"{author.Name} - {string.Join(", ", books.Select(b => b.Title))} [{quality.Quality.Name}]";

            SendNotification(title, body, author);
        }

        public override void OnImportComplete(ImportCompleteMessage message)
        {
            var author = message.Author;
            var book = message.BookFile.Books.Value.First();
            
            var title = message.IsUpgrade ? "Book Upgraded" : "Book Downloaded";
            var body = $"{author.Name} - {book.Title}";

            SendNotification(title, body, author);
        }

        public override void OnRename(Author author, List<RenamedBookFile> renamedFiles)
        {
            var title = "Books Renamed";
            var body = $"{renamedFiles.Count} book files renamed for {author.Name}";

            SendNotification(title, body, author);
        }

        public override void OnAuthorAdd(AuthorAddMessage message)
        {
            var title = "Author Added";
            var body = $"{message.Author.Name}";

            SendNotification(title, body, message.Author);
        }

        public override void OnAuthorDelete(AuthorDeleteMessage message)
        {
            var title = "Author Deleted";
            var body = $"{message.Author.Name}";

            SendNotification(title, body, null);
        }

        public override void OnBookFileDelete(BookFileDeleteMessage message)
        {
            var title = "Book File Deleted";
            var body = $"{message.Author.Name} - {message.Book.Title}";

            SendNotification(title, body, message.Author);
        }

        public override void OnHealthIssue(HealthCheck.HealthCheck healthCheck)
        {
            var title = "Health Issue";
            var body = healthCheck.Message;

            SendNotification(title, body, null, healthCheck.Type);
        }

        public override void OnHealthRestored(HealthCheck.HealthCheck previousCheck)
        {
            var title = "Health Issue Resolved";
            var body = $"The following issue has been resolved: {previousCheck.Message}";

            SendNotification(title, body, null);
        }

        public override void OnApplicationUpdate(ApplicationUpdateMessage updateMessage)
        {
            var title = "Application Updated";
            var body = $"Readarr has been updated to version {updateMessage.NewVersion}";

            SendNotification(title, body, null);
        }

        public override void OnManualInteractionRequired(ManualInteractionRequiredMessage message)
        {
            var author = message.Author;
            var books = message.Book.Books;
            
            var title = "Manual Interaction Required";
            var body = $"{author.Name} - {string.Join(", ", books.Select(b => b.Title))}";

            SendNotification(title, body, author);
        }

        public override ValidationResult Test()
        {
            var failures = new List<ValidationFailure>();

            try
            {
                var isMarkdown = Settings.MetadataLinks && Settings.ContentType == GotifyContentType.Markdown;
                var message = new StringBuilder();
                
                message.AppendLine("This is a test notification from Readarr");
                
                if (isMarkdown)
                {
                    message.AppendLine();
                    message.AppendLine("**Details:**");
                    message.AppendLine("- Notification type: Test");
                    message.AppendLine("- Instance: Readarr");
                }

                _proxy.SendNotification("Test Notification", message.ToString(), Settings);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unable to send test notification");
                failures.Add(new ValidationFailure("Server", _localizationService.GetLocalizedString("NotificationsValidationUnableToSendTestMessage", new Dictionary<string, object> { { "exceptionMessage", ex.Message } })));
            }

            return new ValidationResult(failures);
        }

        private void SendNotification(string title, string message, Author author, HealthCheck.HealthCheckResult? healthCheckResult = null)
        {
            var isMarkdown = Settings.MetadataLinks && Settings.ContentType == GotifyContentType.Markdown;
            var sb = new StringBuilder();

            sb.AppendLine(message);

            if (Settings.MetadataLinks && author != null)
            {
                _logger.Trace("Generating Metadata Links");
                sb.AppendLine();

                if (isMarkdown)
                {
                    sb.AppendLine("**Links:**");
                }

                var links = GetAuthorLinks(author);

                if (isMarkdown && links.Any())
                {
                    foreach (var link in links)
                    {
                        sb.AppendLine($"- [{link.Name}]({link.Url})");
                    }
                }
                else if (links.Any())
                {
                    foreach (var link in links)
                    {
                        sb.AppendLine($"{link.Name}: {link.Url}");
                    }
                }
            }

            var request = new GotifyMessage
            {
                Title = title,
                Message = sb.ToString(),
                Priority = Settings.Priority
            };

            if (healthCheckResult != null)
            {
                request.Priority = healthCheckResult == HealthCheck.HealthCheckResult.Warning ? 
                    Settings.Priority : 
                    Math.Max(Settings.Priority, 8); // High priority for errors
            }

            request.Extras = new GotifyExtras
            {
                Client = new GotifyClientExtras
                {
                    Display = new GotifyClientDisplay
                    {
                        ContentType = Settings.ContentType.ToString().ToLowerInvariant()
                    }
                }
            };

            if (Settings.AppTags.Any())
            {
                request.Extras.Client.Notification = new GotifyClientNotification
                {
                    Bigimageurl = Settings.IncludeAppImage ? ReadarrImageUrl : null,
                    Click = new GotifyClientNotificationClick { Url = $"readarr://view/author/{author?.Id}" }
                };
            }

            _proxy.SendNotification(request, Settings);
        }

        private List<MetadataLink> GetAuthorLinks(Author author)
        {
            var links = new List<MetadataLink>();

            if (author.Metadata.Value.GoodreadsId.IsNotNullOrWhiteSpace())
            {
                links.Add(new MetadataLink
                {
                    Name = "Goodreads",
                    Url = $"https://www.goodreads.com/author/show/{author.Metadata.Value.GoodreadsId}"
                });
            }

            return links;
        }

        private class MetadataLink
        {
            public string Name { get; set; }
            public string Url { get; set; }
        }
    }
}