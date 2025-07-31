using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using Readarr.Common.Extensions;
using Readarr.Core.Books;
using Readarr.Core.Localization;
using Readarr.Core.MediaFiles;
using Readarr.Core.Notifications.Slack.Payloads;
using Readarr.Core.Validation;

namespace Readarr.Core.Notifications.Slack
{
    public class SlackBook : BookNotificationBase<SlackSettings>
    {
        private readonly ISlackProxy _proxy;
        private readonly ILocalizationService _localizationService;

        public SlackBook(ISlackProxy proxy, ILocalizationService localizationService)
        {
            _proxy = proxy;
            _localizationService = localizationService;
        }

        public override string Name => "Slack";
        public override string Link => "https://my.slack.com/services/new/incoming-webhook/";

        public override void OnGrab(GrabMessage message)
        {
            var author = message.Book.Author;
            var books = message.Book.Books;
            var quality = message.Quality;

            var attachments = new List<Attachment>
            {
                new Attachment
                {
                    Fallback = $"{author.Name} - {string.Join(", ", books.Select(b => b.Title))}",
                    Title = author.Name,
                    Text = $"Book Grabbed: {string.Join(", ", books.Select(b => b.Title))} [{quality.Quality.Name}]",
                    Color = "warning"
                }
            };

            var payload = CreatePayload("Book Grabbed", attachments);
            _proxy.SendPayload(payload, Settings);
        }

        public override void OnImportComplete(ImportCompleteMessage message)
        {
            var author = message.Author;
            var book = message.BookFile.Books.Value.First();
            
            var attachments = new List<Attachment>
            {
                new Attachment
                {
                    Fallback = $"{author.Name} - {book.Title}",
                    Title = author.Name,
                    Text = $"{(message.IsUpgrade ? "Book Upgraded" : "Book Downloaded")}: {book.Title}",
                    Color = message.IsUpgrade ? "good" : "good"
                }
            };

            var payload = CreatePayload(message.IsUpgrade ? "Book Upgraded" : "Book Downloaded", attachments);
            _proxy.SendPayload(payload, Settings);
        }

        public override void OnRename(Author author, List<RenamedBookFile> renamedFiles)
        {
            var attachments = new List<Attachment>
            {
                new Attachment
                {
                    Fallback = $"{renamedFiles.Count} files renamed",
                    Title = author.Name,
                    Text = $"{renamedFiles.Count} book files renamed",
                    Color = "good"
                }
            };

            var payload = CreatePayload("Books Renamed", attachments);
            _proxy.SendPayload(payload, Settings);
        }

        public override void OnAuthorAdd(AuthorAddMessage message)
        {
            var attachments = new List<Attachment>
            {
                new Attachment
                {
                    Fallback = message.Author.Name,
                    Title = message.Author.Name,
                    Text = "Author Added",
                    Color = "good"
                }
            };

            var payload = CreatePayload("Author Added", attachments);
            _proxy.SendPayload(payload, Settings);
        }

        public override void OnAuthorDelete(AuthorDeleteMessage message)
        {
            var attachments = new List<Attachment>
            {
                new Attachment
                {
                    Fallback = message.Author.Name,
                    Title = message.Author.Name,
                    Text = message.DeletedFiles ? "Author Deleted With Files" : "Author Deleted",
                    Color = "danger"
                }
            };

            var payload = CreatePayload("Author Deleted", attachments);
            _proxy.SendPayload(payload, Settings);
        }

        public override void OnBookFileDelete(BookFileDeleteMessage message)
        {
            var attachments = new List<Attachment>
            {
                new Attachment
                {
                    Fallback = $"{message.Author.Name} - {message.Book.Title}",
                    Title = message.Author.Name,
                    Text = $"Book File Deleted: {message.Book.Title}",
                    Color = "danger"
                }
            };

            var payload = CreatePayload("Book File Deleted", attachments);
            _proxy.SendPayload(payload, Settings);
        }

        public override void OnHealthIssue(HealthCheck.HealthCheck healthCheck)
        {
            var attachments = new List<Attachment>
            {
                new Attachment
                {
                    Fallback = healthCheck.Message,
                    Title = healthCheck.Source.Name,
                    Text = healthCheck.Message,
                    Color = healthCheck.Type == HealthCheck.HealthCheckResult.Warning ? "warning" : "danger"
                }
            };

            var payload = CreatePayload("Health Issue", attachments);
            _proxy.SendPayload(payload, Settings);
        }

        public override void OnHealthRestored(HealthCheck.HealthCheck previousCheck)
        {
            var attachments = new List<Attachment>
            {
                new Attachment
                {
                    Fallback = $"Health Issue Resolved: {previousCheck.Message}",
                    Title = previousCheck.Source.Name,
                    Text = $"Health Issue Resolved: {previousCheck.Message}",
                    Color = "good"
                }
            };

            var payload = CreatePayload("Health Issue Resolved", attachments);
            _proxy.SendPayload(payload, Settings);
        }

        public override void OnApplicationUpdate(ApplicationUpdateMessage updateMessage)
        {
            var attachments = new List<Attachment>
            {
                new Attachment
                {
                    Fallback = $"Readarr updated to {updateMessage.NewVersion}",
                    Title = "Application Updated",
                    Text = $"Readarr has been updated to version {updateMessage.NewVersion}",
                    Color = "good"
                }
            };

            var payload = CreatePayload("Application Updated", attachments);
            _proxy.SendPayload(payload, Settings);
        }

        public override void OnManualInteractionRequired(ManualInteractionRequiredMessage message)
        {
            var author = message.Author;
            var books = message.Book.Books;
            
            var attachments = new List<Attachment>
            {
                new Attachment
                {
                    Fallback = $"{author.Name} - {string.Join(", ", books.Select(b => b.Title))}",
                    Title = author.Name,
                    Text = $"Manual interaction required for: {string.Join(", ", books.Select(b => b.Title))}",
                    Color = "warning"
                }
            };

            var payload = CreatePayload("Manual Interaction Required", attachments);
            _proxy.SendPayload(payload, Settings);
        }

        public override ValidationResult Test()
        {
            var failures = new List<ValidationFailure>();

            failures.AddIfNotNull(TestMessage());

            return new ValidationResult(failures);
        }

        private ValidationFailure TestMessage()
        {
            try
            {
                var message = $"Test message from Readarr at {DateTime.Now}";
                var payload = CreatePayload(message);

                _proxy.SendPayload(payload, Settings);
            }
            catch (SlackExeption ex)
            {
                return new NzbDroneValidationFailure("Unable to post", _localizationService.GetLocalizedString("NotificationsValidationUnableToSendTestMessage", new Dictionary<string, object> { { "exceptionMessage", ex.Message } }));
            }

            return null;
        }

        private SlackPayload CreatePayload(string text, List<Attachment> attachments = null)
        {
            var icon = Settings.Icon;
            var channel = Settings.Channel;

            var payload = new SlackPayload
            {
                Username = Settings.Username,
                Text = text,
                Attachments = attachments
            };

            if (icon.IsNotNullOrWhiteSpace())
            {
                if (icon.StartsWith(":") && icon.EndsWith(":"))
                {
                    payload.IconEmoji = icon;
                }
                else
                {
                    payload.IconUrl = icon;
                }
            }

            if (channel.IsNotNullOrWhiteSpace())
            {
                payload.Channel = channel;
            }

            return payload;
        }
    }
}