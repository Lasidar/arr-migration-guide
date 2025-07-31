using System.Collections.Generic;
using FluentValidation.Results;
using Readarr.Common.Extensions;
using Readarr.Core.Books;
using Readarr.Core.Configuration;
using Readarr.Core.Localization;
using Readarr.Core.MediaCover;
using Readarr.Core.MediaFiles;
using Readarr.Core.Tags;
using Readarr.Core.Validation;

namespace Readarr.Core.Notifications.Webhook
{
    public class WebhookBookNotification : WebhookBookBase<WebhookSettings>
    {
        private readonly IWebhookProxy _proxy;

        public WebhookBookNotification(IWebhookProxy proxy, 
                          IConfigFileProvider configFileProvider, 
                          IConfigService configService, 
                          ILocalizationService localizationService, 
                          ITagRepository tagRepository, 
                          IMapCoversToLocal mediaCoverService)
            : base(configFileProvider, configService, localizationService, tagRepository, mediaCoverService)
        {
            _proxy = proxy;
        }

        public override string Link => "https://wiki.servarr.com/readarr/settings#connections";

        public override void OnGrab(GrabMessage message)
        {
            _proxy.SendWebhook(BuildOnGrabPayload(message), Settings);
        }

        public override void OnImportComplete(ImportCompleteMessage message)
        {
            _proxy.SendWebhook(BuildOnImportPayload(message), Settings);
        }

        public override void OnRename(Author author, List<RenamedBookFile> renamedFiles)
        {
            _proxy.SendWebhook(BuildOnRenamePayload(author, renamedFiles), Settings);
        }

        public override void OnBookFileDelete(BookFileDeleteMessage deleteMessage)
        {
            _proxy.SendWebhook(BuildOnBookFileDelete(deleteMessage), Settings);
        }

        public override void OnAuthorAdd(AuthorAddMessage message)
        {
            _proxy.SendWebhook(BuildOnAuthorAdd(message), Settings);
        }

        public override void OnAuthorDelete(AuthorDeleteMessage deleteMessage)
        {
            _proxy.SendWebhook(BuildOnAuthorDelete(deleteMessage), Settings);
        }

        public override void OnHealthIssue(HealthCheck.HealthCheck healthCheck)
        {
            var payload = new WebhookHealthPayload
            {
                EventType = WebhookEventType.Health,
                InstanceName = _configFileProvider.InstanceName,
                ApplicationUrl = _configService.ApplicationUrl,
                Level = healthCheck.Type,
                Message = healthCheck.Message,
                Type = healthCheck.Source.Name,
                WikiUrl = healthCheck.WikiUrl?.ToString()
            };

            _proxy.SendWebhook(payload, Settings);
        }

        public override void OnHealthRestored(HealthCheck.HealthCheck previousCheck)
        {
            var payload = new WebhookHealthPayload
            {
                EventType = WebhookEventType.HealthRestored,
                InstanceName = _configFileProvider.InstanceName,
                ApplicationUrl = _configService.ApplicationUrl,
                Level = previousCheck.Type,
                Message = previousCheck.Message,
                Type = previousCheck.Source.Name,
                WikiUrl = previousCheck.WikiUrl?.ToString()
            };

            _proxy.SendWebhook(payload, Settings);
        }

        public override void OnApplicationUpdate(ApplicationUpdateMessage updateMessage)
        {
            var payload = new WebhookApplicationUpdatePayload
            {
                EventType = WebhookEventType.ApplicationUpdate,
                InstanceName = _configFileProvider.InstanceName,
                ApplicationUrl = _configService.ApplicationUrl,
                Message = updateMessage.Message,
                PreviousVersion = updateMessage.PreviousVersion.ToString(),
                NewVersion = updateMessage.NewVersion.ToString()
            };

            _proxy.SendWebhook(payload, Settings);
        }

        public override void OnManualInteractionRequired(ManualInteractionRequiredMessage message)
        {
            var payload = new WebhookManualInteractionRequiredPayload
            {
                EventType = WebhookEventType.ManualInteractionRequired,
                InstanceName = _configFileProvider.InstanceName,
                ApplicationUrl = _configService.ApplicationUrl,
                Author = GetAuthor(message.Author),
                Books = message.Book.Books.ConvertAll(x => new WebhookBook(x)),
                DownloadInfo = new WebhookDownloadInfo
                {
                    DownloadClient = message.DownloadInfo.DownloadClient,
                    DownloadClientType = message.DownloadInfo.DownloadClientType,
                    DownloadId = message.DownloadInfo.DownloadId,
                    DownloadClientName = message.DownloadInfo.Name
                },
                Release = new WebhookRelease(message.Quality, message.Book)
            };

            _proxy.SendWebhook(payload, Settings);
        }

        public override string Name => "Webhook";

        public override ValidationResult Test()
        {
            var failures = new List<ValidationFailure>();

            var payload = new WebhookTestPayload
            {
                EventType = WebhookEventType.Test,
                InstanceName = _configFileProvider.InstanceName,
                ApplicationUrl = _configService.ApplicationUrl
            };

            try
            {
                _proxy.SendWebhook(payload, Settings);
            }
            catch (WebhookException ex)
            {
                failures.Add(new NzbDroneValidationFailure("Url", _localizationService.GetLocalizedString("NotificationsValidationUnableToConnect", new Dictionary<string, object> { { "exceptionMessage", ex.Message } })));
            }

            return new ValidationResult(failures);
        }
    }
}