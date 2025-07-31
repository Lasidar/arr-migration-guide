using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using NLog;
using Readarr.Common.Extensions;
using Readarr.Common.Http.Dispatchers;
using Readarr.Core.Books;
using Readarr.Core.Localization;
using Readarr.Core.MediaFiles;

namespace Readarr.Core.Notifications.Email
{
    public class EmailBook : BookNotificationBase<EmailSettings>
    {
        private readonly ICertificateValidationService _certificateValidationService;
        private readonly ILocalizationService _localizationService;
        private readonly Logger _logger;

        public override string Name => _localizationService.GetLocalizedString("NotificationsEmailSettingsName");

        public EmailBook(ICertificateValidationService certificateValidationService, ILocalizationService localizationService, Logger logger)
        {
            _certificateValidationService = certificateValidationService;
            _localizationService = localizationService;
            _logger = logger;
        }

        public override string Link => null;

        public override void OnGrab(GrabMessage grabMessage)
        {
            var author = grabMessage.Book.Author;
            var books = grabMessage.Book.Books;
            var quality = grabMessage.Quality;

            var subject = "Readarr - Book Grabbed";
            var body = $"{author.Name} - {string.Join(", ", books.Select(b => b.Title))} [{quality.Quality.Name}] sent to download client.";

            SendEmail(Settings, subject, body);
        }

        public override void OnImportComplete(ImportCompleteMessage message)
        {
            var author = message.Author;
            var book = message.BookFile.Books.Value.First();
            
            var subject = message.IsUpgrade ? "Readarr - Book Upgraded" : "Readarr - Book Downloaded";
            var body = $"{author.Name} - {book.Title} has been {(message.IsUpgrade ? "upgraded" : "downloaded")} and sorted.";

            SendEmail(Settings, subject, body);
        }

        public override void OnRename(Author author, List<RenamedBookFile> renamedFiles)
        {
            var subject = "Readarr - Books Renamed";
            var body = $"{renamedFiles.Count} book files renamed for {author.Name}.";

            SendEmail(Settings, subject, body);
        }

        public override void OnAuthorAdd(AuthorAddMessage message)
        {
            var subject = "Readarr - Author Added";
            var body = $"{message.Author.Name} has been added to your library.";

            SendEmail(Settings, subject, body);
        }

        public override void OnAuthorDelete(AuthorDeleteMessage message)
        {
            var subject = "Readarr - Author Deleted";
            var body = $"{message.Author.Name} has been deleted from your library{(message.DeletedFiles ? " with all files" : "")}.";

            SendEmail(Settings, subject, body);
        }

        public override void OnBookFileDelete(BookFileDeleteMessage message)
        {
            var subject = "Readarr - Book File Deleted";
            var body = $"{message.Author.Name} - {message.Book.Title} has been deleted.";

            SendEmail(Settings, subject, body);
        }

        public override void OnHealthIssue(HealthCheck.HealthCheck healthCheck)
        {
            var subject = "Readarr - Health Issue";
            var body = $"Health Issue: {healthCheck.Message}";

            SendEmail(Settings, subject, body);
        }

        public override void OnHealthRestored(HealthCheck.HealthCheck previousCheck)
        {
            var subject = "Readarr - Health Issue Resolved";
            var body = $"The following health issue has been resolved: {previousCheck.Message}";

            SendEmail(Settings, subject, body);
        }

        public override void OnApplicationUpdate(ApplicationUpdateMessage updateMessage)
        {
            var subject = "Readarr - Application Updated";
            var body = $"Readarr has been updated to version {updateMessage.NewVersion}.";

            SendEmail(Settings, subject, body);
        }

        public override void OnManualInteractionRequired(ManualInteractionRequiredMessage message)
        {
            var author = message.Author;
            var books = message.Book.Books;
            
            var subject = "Readarr - Manual Interaction Required";
            var body = $"Manual interaction is required for {author.Name} - {string.Join(", ", books.Select(b => b.Title))}.";

            SendEmail(Settings, subject, body);
        }

        public override ValidationResult Test()
        {
            var failures = new List<ValidationFailure>();

            failures.AddIfNotNull(Test(Settings));

            return new ValidationResult(failures);
        }

        private ValidationFailure Test(EmailSettings settings)
        {
            const string body = "Success! You have properly configured your email notification settings";

            try
            {
                SendEmail(settings, "Readarr - Test Notification", body);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unable to send test email");
                return new ValidationFailure("Server", _localizationService.GetLocalizedString("NotificationsValidationUnableToSendTestMessage", new Dictionary<string, object> { { "exceptionMessage", ex.Message } }));
            }

            return null;
        }

        private void SendEmail(EmailSettings settings, string subject, string body, bool htmlBody = false)
        {
            var email = new MimeMessage();

            email.From.Add(ParseAddress("From", settings.From));
            email.To.AddRange(settings.To.Select(x => ParseAddress("To", x)));
            email.Cc.AddRange(settings.Cc.Select(x => ParseAddress("CC", x)));
            email.Bcc.AddRange(settings.Bcc.Select(x => ParseAddress("BCC", x)));

            email.Subject = subject;
            email.Body = new TextPart(htmlBody ? "html" : "plain")
            {
                Text = body
            };

            _logger.Debug("Sending email Subject: {0}", subject);

            try
            {
                Send(email, settings);
                _logger.Debug("Email sent. Subject: {0}", subject);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error sending email. Subject: {0}", subject);
                throw;
            }

            _logger.Debug("Finished sending email. Subject: {0}", subject);
        }

        private void Send(MimeMessage email, EmailSettings settings)
        {
            using var client = new SmtpClient();
            
            client.Timeout = 10000;

            var serverOption = SecureSocketOptions.Auto;

            if (settings.RequireEncryption)
            {
                if (settings.Port == 465)
                {
                    serverOption = SecureSocketOptions.SslOnConnect;
                }
                else
                {
                    serverOption = SecureSocketOptions.StartTls;
                }
            }

            client.ServerCertificateValidationCallback = _certificateValidationService.ShouldByPassValidationError;

            _logger.Debug("Connecting to mail server");

            client.Connect(settings.Server, settings.Port, serverOption);

            if (!string.IsNullOrWhiteSpace(settings.Username))
            {
                _logger.Debug("Authenticating to mail server");

                client.Authenticate(settings.Username, settings.Password);
            }

            _logger.Debug("Sending to mail server");

            client.Send(email);

            _logger.Debug("Sent to mail server, disconnecting");

            client.Disconnect(true);

            _logger.Debug("Disconnected from mail server");
        }

        private MailboxAddress ParseAddress(string type, string address)
        {
            try
            {
                return MailboxAddress.Parse(address);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "{0} email address '{1}' invalid", type, address);
                throw;
            }
        }
    }
}