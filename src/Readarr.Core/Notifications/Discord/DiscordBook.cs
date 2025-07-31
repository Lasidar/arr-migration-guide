using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentValidation.Results;
using Readarr.Common.Extensions;
using Readarr.Core.Books;
using Readarr.Core.Configuration;
using Readarr.Core.Localization;
using Readarr.Core.MediaCover;
using Readarr.Core.MediaFiles;
using Readarr.Core.Notifications.Discord.Payloads;
using Readarr.Core.Validation;

namespace Readarr.Core.Notifications.Discord
{
    public class DiscordBook : BookNotificationBase<DiscordSettings>
    {
        private readonly IDiscordProxy _proxy;
        private readonly IConfigFileProvider _configFileProvider;
        private readonly ILocalizationService _localizationService;

        public DiscordBook(IDiscordProxy proxy, IConfigFileProvider configFileProvider, ILocalizationService localizationService)
        {
            _proxy = proxy;
            _configFileProvider = configFileProvider;
            _localizationService = localizationService;
        }

        public override string Name => "Discord";
        public override string Link => "https://support.discordapp.com/hc/en-us/articles/228383668-Intro-to-Webhooks";

        public override void OnGrab(GrabMessage message)
        {
            var author = message.Book.Author;
            var books = message.Book.Books;

            var embed = new Embed
            {
                Author = new DiscordAuthor
                {
                    Name = Settings.Author.IsNullOrWhiteSpace() ? _configFileProvider.InstanceName : Settings.Author,
                    IconUrl = "https://raw.githubusercontent.com/Readarr/Readarr/develop/Logo/256.png"
                },
                Url = $"https://www.goodreads.com/author/show/{author.Metadata.Value.GoodreadsId}",
                Description = "Book Grabbed",
                Title = GetTitle(author, books),
                Color = (int)DiscordColors.Standard,
                Fields = new List<DiscordField>(),
                Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            };

            if (Settings.GrabFields.Contains((int)DiscordGrabFieldType.Poster))
            {
                embed.Thumbnail = new DiscordImage
                {
                    Url = GetCoverUrl(author)
                };
            }

            foreach (var field in Settings.GrabFields)
            {
                var discordField = new DiscordField();

                switch ((DiscordGrabFieldType)field)
                {
                    case DiscordGrabFieldType.Overview:
                        var overview = books.First().Overview ?? "";
                        discordField.Name = "Overview";
                        discordField.Value = overview.Length <= 300 ? overview : $"{overview.AsSpan(0, 300)}...";
                        break;
                    case DiscordGrabFieldType.Rating:
                        discordField.Name = "Rating";
                        discordField.Value = books.First().Ratings?.Value?.ToString() ?? "N/A";
                        break;
                    case DiscordGrabFieldType.Genres:
                        discordField.Name = "Genres";
                        discordField.Value = books.First().Genres?.Any() == true ? string.Join(", ", books.First().Genres) : "N/A";
                        break;
                    case DiscordGrabFieldType.Quality:
                        discordField.Name = "Quality";
                        discordField.Value = message.Quality.Quality.Name;
                        break;
                    case DiscordGrabFieldType.Size:
                        discordField.Name = "Size";
                        discordField.Value = BytesToString(message.Book.Release.Size);
                        break;
                    case DiscordGrabFieldType.Release:
                        discordField.Name = "Release";
                        discordField.Value = message.Book.Release.Title;
                        break;
                    case DiscordGrabFieldType.Links:
                        discordField.Name = "Links";
                        discordField.Value = GetLinksString(author);
                        break;
                }

                if (discordField.Name.IsNotNullOrWhiteSpace() && discordField.Value.IsNotNullOrWhiteSpace())
                {
                    embed.Fields.Add(discordField);
                }
            }

            var payload = CreatePayload(null, new List<Embed> { embed });

            _proxy.SendPayload(payload, Settings);
        }

        public override void OnImportComplete(ImportCompleteMessage message)
        {
            var attachments = new List<Embed>
            {
                new Embed
                {
                    Author = new DiscordAuthor
                    {
                        Name = Settings.Author.IsNullOrWhiteSpace() ? _configFileProvider.InstanceName : Settings.Author,
                        IconUrl = "https://raw.githubusercontent.com/Readarr/Readarr/develop/Logo/256.png"
                    },
                    Url = $"https://www.goodreads.com/author/show/{message.Author.Metadata.Value.GoodreadsId}",
                    Description = message.IsUpgrade ? "Book Upgraded" : "Book Imported",
                    Title = GetTitle(message.Author, message.BookFile.Books.Value),
                    Color = message.IsUpgrade ? (int)DiscordColors.Upgrade : (int)DiscordColors.Success,
                    Fields = new List<DiscordField>(),
                    Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                }
            };

            var payload = CreatePayload(null, attachments);

            _proxy.SendPayload(payload, Settings);
        }

        public override void OnRename(Author author, List<RenamedBookFile> renamedFiles)
        {
            var attachments = new List<Embed>
            {
                new Embed
                {
                    Author = new DiscordAuthor
                    {
                        Name = Settings.Author.IsNullOrWhiteSpace() ? _configFileProvider.InstanceName : Settings.Author,
                        IconUrl = "https://raw.githubusercontent.com/Readarr/Readarr/develop/Logo/256.png"
                    },
                    Title = author.Name,
                    Description = "Renamed",
                    Color = (int)DiscordColors.Success,
                    Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                }
            };

            var payload = CreatePayload(null, attachments);

            _proxy.SendPayload(payload, Settings);
        }

        public override void OnAuthorAdd(AuthorAddMessage message)
        {
            var attachments = new List<Embed>
            {
                new Embed
                {
                    Author = new DiscordAuthor
                    {
                        Name = Settings.Author.IsNullOrWhiteSpace() ? _configFileProvider.InstanceName : Settings.Author,
                        IconUrl = "https://raw.githubusercontent.com/Readarr/Readarr/develop/Logo/256.png"
                    },
                    Title = message.Author.Name,
                    Description = "Author Added",
                    Url = $"https://www.goodreads.com/author/show/{message.Author.Metadata.Value.GoodreadsId}",
                    Color = (int)DiscordColors.Success,
                    Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                }
            };

            var payload = CreatePayload(null, attachments);

            _proxy.SendPayload(payload, Settings);
        }

        public override void OnAuthorDelete(AuthorDeleteMessage message)
        {
            var attachments = new List<Embed>
            {
                new Embed
                {
                    Author = new DiscordAuthor
                    {
                        Name = Settings.Author.IsNullOrWhiteSpace() ? _configFileProvider.InstanceName : Settings.Author,
                        IconUrl = "https://raw.githubusercontent.com/Readarr/Readarr/develop/Logo/256.png"
                    },
                    Title = message.Author.Name,
                    Description = message.DeletedFiles ? "Author Deleted With Files" : "Author Deleted",
                    Color = (int)DiscordColors.Danger,
                    Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                }
            };

            var payload = CreatePayload(null, attachments);

            _proxy.SendPayload(payload, Settings);
        }

        public override void OnBookFileDelete(BookFileDeleteMessage message)
        {
            var attachments = new List<Embed>
            {
                new Embed
                {
                    Author = new DiscordAuthor
                    {
                        Name = Settings.Author.IsNullOrWhiteSpace() ? _configFileProvider.InstanceName : Settings.Author,
                        IconUrl = "https://raw.githubusercontent.com/Readarr/Readarr/develop/Logo/256.png"
                    },
                    Title = GetTitle(message.Author, new List<Book> { message.Book }),
                    Description = "Book File Deleted",
                    Color = (int)DiscordColors.Danger,
                    Fields = new List<DiscordField>
                    {
                        new DiscordField
                        {
                            Name = "Reason",
                            Value = message.Reason.ToString()
                        }
                    },
                    Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                }
            };

            var payload = CreatePayload(null, attachments);

            _proxy.SendPayload(payload, Settings);
        }

        public override void OnHealthIssue(HealthCheck.HealthCheck healthCheck)
        {
            var attachments = new List<Embed>
            {
                new Embed
                {
                    Author = new DiscordAuthor
                    {
                        Name = Settings.Author.IsNullOrWhiteSpace() ? _configFileProvider.InstanceName : Settings.Author,
                        IconUrl = "https://raw.githubusercontent.com/Readarr/Readarr/develop/Logo/256.png"
                    },
                    Title = healthCheck.Source.Name,
                    Description = healthCheck.Message,
                    Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    Color = healthCheck.Type == HealthCheck.HealthCheckResult.Warning ? (int)DiscordColors.Warning : (int)DiscordColors.Danger
                }
            };

            var payload = CreatePayload(null, attachments);

            _proxy.SendPayload(payload, Settings);
        }

        public override void OnHealthRestored(HealthCheck.HealthCheck previousCheck)
        {
            var attachments = new List<Embed>
            {
                new Embed
                {
                    Author = new DiscordAuthor
                    {
                        Name = Settings.Author.IsNullOrWhiteSpace() ? _configFileProvider.InstanceName : Settings.Author,
                        IconUrl = "https://raw.githubusercontent.com/Readarr/Readarr/develop/Logo/256.png"
                    },
                    Title = "Health Issue Resolved",
                    Description = $"The following health issue has been resolved:\n{previousCheck.Message}",
                    Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    Color = (int)DiscordColors.Success
                }
            };

            var payload = CreatePayload(null, attachments);

            _proxy.SendPayload(payload, Settings);
        }

        public override void OnApplicationUpdate(ApplicationUpdateMessage updateMessage)
        {
            var attachments = new List<Embed>
            {
                new Embed
                {
                    Author = new DiscordAuthor
                    {
                        Name = Settings.Author.IsNullOrWhiteSpace() ? _configFileProvider.InstanceName : Settings.Author,
                        IconUrl = "https://raw.githubusercontent.com/Readarr/Readarr/develop/Logo/256.png"
                    },
                    Title = "Application Updated",
                    Description = $"Readarr has been updated to version {updateMessage.NewVersion}",
                    Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    Color = (int)DiscordColors.Standard
                }
            };

            var payload = CreatePayload(null, attachments);

            _proxy.SendPayload(payload, Settings);
        }

        public override void OnManualInteractionRequired(ManualInteractionRequiredMessage message)
        {
            var attachments = new List<Embed>
            {
                new Embed
                {
                    Author = new DiscordAuthor
                    {
                        Name = Settings.Author.IsNullOrWhiteSpace() ? _configFileProvider.InstanceName : Settings.Author,
                        IconUrl = "https://raw.githubusercontent.com/Readarr/Readarr/develop/Logo/256.png"
                    },
                    Title = GetTitle(message.Author, message.Book.Books),
                    Description = "Manual interaction required",
                    Color = (int)DiscordColors.Warning,
                    Fields = new List<DiscordField>
                    {
                        new DiscordField
                        {
                            Name = "Client",
                            Value = message.DownloadInfo.DownloadClient
                        },
                        new DiscordField
                        {
                            Name = "ID",
                            Value = message.DownloadInfo.DownloadId
                        }
                    },
                    Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                }
            };

            var payload = CreatePayload(null, attachments);

            _proxy.SendPayload(payload, Settings);
        }

        public override ValidationResult Test()
        {
            var failures = new List<ValidationFailure>();

            failures.AddIfNotNull(TestMessage());

            return new ValidationResult(failures);
        }

        public ValidationFailure TestMessage()
        {
            try
            {
                var message = $"Test message from Readarr at {DateTime.Now}";
                var payload = CreatePayload(message);

                _proxy.SendPayload(payload, Settings);

            }
            catch (DiscordException ex)
            {
                return new NzbDroneValidationFailure("WebHookUrl", _localizationService.GetLocalizedString("NotificationsValidationUnableToConnect", new Dictionary<string, object> { { "exceptionMessage", ex.Message } }));
            }

            return null;
        }

        private DiscordPayload CreatePayload(string message, List<Embed> embeds = null)
        {
            var avatar = Settings.Avatar;

            var payload = new DiscordPayload
            {
                Username = Settings.Username,
                Content = message,
                Embeds = embeds
            };

            if (avatar.IsNotNullOrWhiteSpace())
            {
                payload.AvatarUrl = avatar;
            }

            if (Settings.Username.IsNotNullOrWhiteSpace())
            {
                payload.Username = Settings.Username;
            }

            return payload;
        }

        private string GetTitle(Author author, List<Book> books)
        {
            if (books.Count == 1)
            {
                return $"{author.Name} - {books.First().Title}";
            }

            return $"{author.Name} - {books.Count} books";
        }

        private string GetCoverUrl(Author author)
        {
            var covers = author.Metadata.Value.Images;
            var cover = covers.FirstOrDefault(c => c.CoverType == MediaCoverTypes.Poster);

            if (cover != null)
            {
                return cover.RemoteUrl;
            }

            return null;
        }

        private string GetLinksString(Author author)
        {
            var links = new List<string>();

            if (author.Metadata.Value.GoodreadsId.IsNotNullOrWhiteSpace())
            {
                links.Add($"[Goodreads](https://www.goodreads.com/author/show/{author.Metadata.Value.GoodreadsId})");
            }

            return string.Join(" | ", links);
        }

        private static string BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            if (byteCount == 0)
            {
                return "0 " + suf[0];
            }

            var bytes = Math.Abs(byteCount);
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            var num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return string.Format(CultureInfo.InvariantCulture, "{0} {1}", (Math.Sign(byteCount) * num).ToString(), suf[place]);
        }
    }
}