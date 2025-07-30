using System.Collections.Generic;
using System.IO;
using System.Linq;
using Readarr.Common.Extensions;
using Readarr.Core.Books;
using Readarr.Core.Configuration;
using Readarr.Core.Localization;
using Readarr.Core.MediaCover;
using Readarr.Core.MediaFiles;
using Readarr.Core.Tags;

namespace Readarr.Core.Notifications.Webhook
{
    public abstract class WebhookBookBase<TSettings> : BookNotificationBase<TSettings>
        where TSettings : NotificationSettingsBase<TSettings>, new()
    {
        private readonly IConfigFileProvider _configFileProvider;
        private readonly IConfigService _configService;
        protected readonly ILocalizationService _localizationService;
        private readonly ITagRepository _tagRepository;
        private readonly IMapCoversToLocal _mediaCoverService;

        protected WebhookBookBase(IConfigFileProvider configFileProvider, IConfigService configService, ILocalizationService localizationService, ITagRepository tagRepository, IMapCoversToLocal mediaCoverService)
        {
            _configFileProvider = configFileProvider;
            _configService = configService;
            _localizationService = localizationService;
            _tagRepository = tagRepository;
            _mediaCoverService = mediaCoverService;
        }

        protected WebhookBookGrabPayload BuildOnGrabPayload(GrabMessage message)
        {
            var remoteBook = message.Book;
            var quality = message.Quality;

            return new WebhookBookGrabPayload
            {
                EventType = WebhookEventType.Grab,
                InstanceName = _configFileProvider.InstanceName,
                ApplicationUrl = _configService.ApplicationUrl,
                Author = GetAuthor(remoteBook.Author),
                Books = remoteBook.Books.ConvertAll(x => new WebhookBook(x)),
                Release = new WebhookRelease(quality, remoteBook),
                DownloadClient = message.DownloadClientName,
                DownloadClientType = message.DownloadClientType,
                DownloadId = message.DownloadId,
                CustomFormatInfo = new WebhookCustomFormatInfo(remoteBook.CustomFormats, remoteBook.CustomFormatScore),
            };
        }

        protected WebhookBookImportPayload BuildOnImportPayload(ImportCompleteMessage message)
        {
            var bookFile = message.BookFile;

            return new WebhookBookImportPayload
            {
                EventType = WebhookEventType.Download,
                InstanceName = _configFileProvider.InstanceName,
                ApplicationUrl = _configService.ApplicationUrl,
                Author = GetAuthor(bookFile.Author.Value),
                Books = bookFile.Books.Value.ConvertAll(x => new WebhookBook(x)),
                BookFile = new WebhookBookFile(bookFile),
                IsUpgrade = message.IsUpgrade,
                DownloadClient = message.DownloadClientInfo?.Name,
                DownloadId = message.DownloadId,
                DeletedFiles = message.OldFiles.ConvertAll(x => new WebhookBookFile(x.BookFile))
            };
        }

        protected WebhookBookFileDeletePayload BuildOnBookFileDelete(BookFileDeleteMessage deleteMessage)
        {
            return new WebhookBookFileDeletePayload
            {
                EventType = WebhookEventType.BookFileDelete,
                InstanceName = _configFileProvider.InstanceName,
                ApplicationUrl = _configService.ApplicationUrl,
                Author = GetAuthor(deleteMessage.Author),
                Book = new WebhookBook(deleteMessage.Book),
                BookFile = new WebhookBookFile(deleteMessage.BookFile),
                DeleteReason = deleteMessage.Reason
            };
        }

        protected WebhookAuthorAddPayload BuildOnAuthorAdd(AuthorAddMessage addMessage)
        {
            return new WebhookAuthorAddPayload
            {
                EventType = WebhookEventType.AuthorAdd,
                InstanceName = _configFileProvider.InstanceName,
                ApplicationUrl = _configService.ApplicationUrl,
                Author = GetAuthor(addMessage.Author)
            };
        }

        protected WebhookAuthorDeletePayload BuildOnAuthorDelete(AuthorDeleteMessage deleteMessage)
        {
            return new WebhookAuthorDeletePayload
            {
                EventType = WebhookEventType.AuthorDelete,
                InstanceName = _configFileProvider.InstanceName,
                ApplicationUrl = _configService.ApplicationUrl,
                Author = GetAuthor(deleteMessage.Author),
                DeletedFiles = deleteMessage.DeletedFiles
            };
        }

        protected WebhookRenamePayload BuildOnRenamePayload(Author author, List<RenamedBookFile> renamedFiles)
        {
            return new WebhookRenamePayload
            {
                EventType = WebhookEventType.Rename,
                InstanceName = _configFileProvider.InstanceName,
                ApplicationUrl = _configService.ApplicationUrl,
                Author = GetAuthor(author),
                RenamedBookFiles = renamedFiles.ConvertAll(x => new WebhookRenamedBookFile(x))
            };
        }

        private WebhookAuthor GetAuthor(Author author)
        {
            if (author == null)
            {
                return null;
            }

            var webhook = new WebhookAuthor(author);
            webhook.Tags = GetTagLabels(author);
            
            return webhook;
        }

        private List<string> GetTagLabels(Author author)
        {
            return _tagRepository.GetTags(author.Tags)
                .Select(t => t.Label)
                .Where(l => l.IsNotNullOrWhiteSpace())
                .OrderBy(l => l)
                .ToList();
        }
    }
}