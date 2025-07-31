using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Readarr.Common.Reflection;
using Readarr.Core.Authentication;
using Readarr.Core.AutoTagging.Specifications;
using Readarr.Core.Blocklisting;
using Readarr.Core.Configuration;
using Readarr.Core.CustomFilters;
using Readarr.Core.CustomFormats;
using Readarr.Core.DataAugmentation.Scene;
using Readarr.Core.Datastore.Converters;
using Readarr.Core.Download;
using Readarr.Core.Download.History;
using Readarr.Core.Download.Pending;
using Readarr.Core.Extras.Metadata;
using Readarr.Core.Extras.Metadata.Files;
using Readarr.Core.Extras.Others;
using Readarr.Core.Extras.Subtitles;
using Readarr.Core.History;
using Readarr.Core.ImportLists;
using Readarr.Core.ImportLists.Exclusions;
using Readarr.Core.Indexers;
using Readarr.Core.Instrumentation;
using Readarr.Core.Jobs;
using Readarr.Core.Languages;
using Readarr.Core.MediaFiles;
using Readarr.Core.Messaging.Commands;
using Readarr.Core.Notifications;
using Readarr.Core.Organizer;
using Readarr.Core.Parser.Model;
using Readarr.Core.Profiles;
using Readarr.Core.Profiles.Delay;
using Readarr.Core.Profiles.Qualities;
using Readarr.Core.Profiles.Releases;
using Readarr.Core.Qualities;
using Readarr.Core.RemotePathMappings;
using Readarr.Core.RootFolders;
using Readarr.Core.Tags;
using Readarr.Core.ThingiProvider;
using Readarr.Core.Books;
using Readarr.Core.Books;
using Readarr.Core.Update.History;
using static Dapper.SqlMapper;

namespace Readarr.Core.Datastore
{
    public static class TableMapping
    {
        static TableMapping()
        {
            Mapper = new TableMapper();
        }

        public static TableMapper Mapper { get; private set; }

        public static void Map()
        {
            RegisterMappers();

            Mapper.Entity<Config>("Config").RegisterModel();

            Mapper.Entity<RootFolder>("RootFolders").RegisterModel()
                  .Ignore(r => r.Accessible)
                  .Ignore(r => r.FreeSpace)
                  .Ignore(r => r.TotalSpace);

            Mapper.Entity<ScheduledTask>("ScheduledTasks").RegisterModel()
                  .Ignore(i => i.Priority);

            Mapper.Entity<IndexerDefinition>("Indexers").RegisterModel()
                  .Ignore(x => x.ImplementationName)
                  .Ignore(i => i.Enable)
                  .Ignore(i => i.Protocol)
                  .Ignore(i => i.SupportsRss)
                  .Ignore(i => i.SupportsSearch);

            Mapper.Entity<ImportListDefinition>("ImportLists").RegisterModel()
                  .Ignore(x => x.ImplementationName)
                  .Ignore(i => i.ListType)
                  .Ignore(i => i.MinRefreshInterval)
                  .Ignore(i => i.Enable);

            Mapper.Entity<ImportListItemInfo>("ImportListItems").RegisterModel()
                   .Ignore(i => i.ImportList)
                   .Ignore(i => i.Seasons);

            Mapper.Entity<NotificationDefinition>("Notifications").RegisterModel()
                  .Ignore(x => x.ImplementationName)
                  .Ignore(i => i.SupportsOnGrab)
                  .Ignore(i => i.SupportsOnDownload)
                  .Ignore(i => i.SupportsOnImportComplete)
                  .Ignore(i => i.SupportsOnUpgrade)
                  .Ignore(i => i.SupportsOnRename)
                  .Ignore(i => i.SupportsOnSeriesAdd)
                  .Ignore(i => i.SupportsOnSeriesDelete)
                  .Ignore(i => i.SupportsOnEpisodeFileDelete)
                  .Ignore(i => i.SupportsOnEpisodeFileDeleteForUpgrade)
                  .Ignore(i => i.SupportsOnHealthIssue)
                  .Ignore(i => i.SupportsOnHealthRestored)
                  .Ignore(i => i.SupportsOnApplicationUpdate)
                  .Ignore(i => i.SupportsOnManualInteractionRequired);

            Mapper.Entity<MetadataDefinition>("Metadata").RegisterModel()
                  .Ignore(x => x.ImplementationName)
                  .Ignore(d => d.Tags);

            Mapper.Entity<DownloadClientDefinition>("DownloadClients").RegisterModel()
                  .Ignore(x => x.ImplementationName)
                  .Ignore(d => d.Protocol);

            Mapper.Entity<SceneMapping>("SceneMappings").RegisterModel();

            Mapper.Entity<BookHistory>("History").RegisterModel();

            // Book domain mappings
            Mapper.Entity<Author>("Authors").RegisterModel()
                  .Ignore(a => a.RootFolderPath)
                  .HasOne(a => a.QualityProfile, a => a.QualityProfileId)
                  .LazyLoad(a => a.Metadata, 
                            (db, parent) => db.Query<AuthorMetadata>(new SqlBuilder(db.DatabaseType).Where<AuthorMetadata>(m => m.Id == parent.AuthorMetadataId)).SingleOrDefault(),
                            a => a.AuthorMetadataId > 0)
                  .LazyLoad(a => a.Books,
                            (db, parent) => db.Query<Book>(new SqlBuilder(db.DatabaseType).Where<Book>(b => b.AuthorId == parent.Id)).ToList(),
                            a => a.Id > 0)
                  .LazyLoad(a => a.Series,
                            (db, parent) => db.Query<Books.Series>(new SqlBuilder(db.DatabaseType).Where<Books.Series>(s => s.AuthorId == parent.Id)).ToList(),
                            a => a.Id > 0);

            Mapper.Entity<AuthorMetadata>("AuthorMetadata").RegisterModel();

            Mapper.Entity<Book>("Books").RegisterModel()
                  .LazyLoad(b => b.Author,
                            (db, parent) => db.Query<Author>(new SqlBuilder(db.DatabaseType).Where<Author>(a => a.Id == parent.AuthorId)).SingleOrDefault(),
                            b => b.AuthorId > 0)
                  .LazyLoad(b => b.Metadata,
                            (db, parent) => db.Query<BookMetadata>(new SqlBuilder(db.DatabaseType).Where<BookMetadata>(m => m.Id == parent.BookMetadataId)).SingleOrDefault(),
                            b => b.BookMetadataId > 0)
                  .LazyLoad(b => b.Editions,
                            (db, parent) => db.Query<Edition>(new SqlBuilder(db.DatabaseType).Where<Edition>(e => e.BookId == parent.Id)).ToList(),
                            b => b.Id > 0);

            Mapper.Entity<BookMetadata>("BookMetadata").RegisterModel();

            Mapper.Entity<Edition>("Editions").RegisterModel()
                  .Ignore(e => e.HasFile)
                  .LazyLoad(e => e.Book,
                            (db, parent) => db.Query<Book>(new SqlBuilder(db.DatabaseType).Where<Book>(b => b.Id == parent.BookId)).SingleOrDefault(),
                            e => e.BookId > 0)
                  .LazyLoad(e => e.BookFile,
                            (db, parent) => db.Query<BookFile>(new SqlBuilder(db.DatabaseType).Where<BookFile>(f => f.Id == parent.BookFileId)).SingleOrDefault(),
                            e => e.BookFileId > 0);

            Mapper.Entity<Books.Series>("Series").RegisterModel()
                  .LazyLoad(s => s.Author,
                            (db, parent) => db.Query<Author>(new SqlBuilder(db.DatabaseType).Where<Author>(a => a.Id == parent.AuthorId)).SingleOrDefault(),
                            s => s.AuthorId > 0)
                  .LazyLoad(s => s.Books,
                            (db, parent) => db.Query<SeriesBookLink>(new SqlBuilder(db.DatabaseType).Where<SeriesBookLink>(l => l.SeriesId == parent.Id)).ToList(),
                            s => s.Id > 0);

            Mapper.Entity<SeriesBookLink>("SeriesBookLink").RegisterModel()
                  .LazyLoad(l => l.Series,
                            (db, parent) => db.Query<Books.Series>(new SqlBuilder(db.DatabaseType).Where<Books.Series>(s => s.Id == parent.SeriesId)).SingleOrDefault(),
                            l => l.SeriesId > 0)
                  .LazyLoad(l => l.Book,
                            (db, parent) => db.Query<Book>(new SqlBuilder(db.DatabaseType).Where<Book>(b => b.Id == parent.BookId)).SingleOrDefault(),
                            l => l.BookId > 0);

            Mapper.Entity<BookFile>("BookFiles").RegisterModel()
                  .HasOne(f => f.Author, f => f.AuthorId)
                  .HasOne(f => f.Book, f => f.BookId)
                  .LazyLoad(f => f.Edition,
                            (db, parent) => db.Query<Edition>(new SqlBuilder(db.DatabaseType).Where<Edition>(e => e.BookFileId == parent.Id)).SingleOrDefault(),
                            f => f.Id > 0)
                  .Ignore(f => f.Path);

            // Legacy TV mappings removed - TV content not supported

            // Episode mapping removed - TV content not supported

            Mapper.Entity<QualityDefinition>("QualityDefinitions").RegisterModel()
                  .Ignore(d => d.GroupName)
                  .Ignore(d => d.Weight)
                  .Ignore(d => d.MinSize)
                  .Ignore(d => d.MaxSize)
                  .Ignore(d => d.PreferredSize);

            Mapper.Entity<CustomFormat>("CustomFormats").RegisterModel();

            Mapper.Entity<QualityProfile>("QualityProfiles").RegisterModel();
            Mapper.Entity<Log>("Logs").RegisterModel();
            Mapper.Entity<NamingConfig>("NamingConfig").RegisterModel();
            Mapper.Entity<Blocklist>("Blocklist").RegisterModel();
            Mapper.Entity<MetadataFile>("MetadataFiles").RegisterModel();
            Mapper.Entity<SubtitleFile>("SubtitleFiles").RegisterModel();
            Mapper.Entity<OtherExtraFile>("ExtraFiles").RegisterModel();

            Mapper.Entity<PendingRelease>("PendingReleases").RegisterModel()
                  .Ignore(e => e.RemoteEpisode);

            Mapper.Entity<RemotePathMapping>("RemotePathMappings").RegisterModel();
            Mapper.Entity<Tag>("Tags").RegisterModel();
            Mapper.Entity<ReleaseProfile>("ReleaseProfiles").RegisterModel();

            Mapper.Entity<DelayProfile>("DelayProfiles").RegisterModel();
            Mapper.Entity<User>("Users").RegisterModel();
            Mapper.Entity<CommandModel>("Commands").RegisterModel()
                .Ignore(c => c.Message);

            Mapper.Entity<IndexerStatus>("IndexerStatus").RegisterModel();
            Mapper.Entity<DownloadClientStatus>("DownloadClientStatus").RegisterModel();
            Mapper.Entity<ImportListStatus>("ImportListStatus").RegisterModel();
            Mapper.Entity<NotificationStatus>("NotificationStatus").RegisterModel();

            Mapper.Entity<CustomFilter>("CustomFilters").RegisterModel();

            Mapper.Entity<DownloadHistory>("DownloadHistory").RegisterModel();

            Mapper.Entity<UpdateHistory>("UpdateHistory").RegisterModel();
            Mapper.Entity<ImportListExclusion>("ImportListExclusions").RegisterModel();

            Mapper.Entity<AutoTagging.AutoTag>("AutoTagging").RegisterModel();
        }

        private static void RegisterMappers()
        {
            RegisterEmbeddedConverter();
            RegisterProviderSettingConverter();

            SqlMapper.RemoveTypeMap(typeof(DateTime));
            SqlMapper.AddTypeHandler(new DapperUtcConverter());
            SqlMapper.AddTypeHandler(new DapperQualityIntConverter());
            SqlMapper.AddTypeHandler(new EmbeddedDocumentConverter<List<QualityProfileQualityItem>>(new QualityIntConverter()));
            SqlMapper.AddTypeHandler(new EmbeddedDocumentConverter<List<ProfileFormatItem>>(new CustomFormatIntConverter()));
            SqlMapper.AddTypeHandler(new EmbeddedDocumentConverter<List<ICustomFormatSpecification>>(new CustomFormatSpecificationListConverter()));
            SqlMapper.AddTypeHandler(new EmbeddedDocumentConverter<List<IAutoTaggingSpecification>>(new AutoTaggingSpecificationConverter()));
            SqlMapper.AddTypeHandler(new EmbeddedDocumentConverter<QualityModel>(new QualityIntConverter()));
            SqlMapper.AddTypeHandler(new EmbeddedDocumentConverter<Dictionary<string, string>>());
            SqlMapper.AddTypeHandler(new EmbeddedDocumentConverter<IDictionary<string, string>>());
            SqlMapper.AddTypeHandler(new EmbeddedDocumentConverter<List<int>>());
            SqlMapper.AddTypeHandler(new EmbeddedDocumentConverter<List<KeyValuePair<string, int>>>());
            SqlMapper.AddTypeHandler(new EmbeddedDocumentConverter<KeyValuePair<string, int>>());
            SqlMapper.AddTypeHandler(new DapperLanguageIntConverter());
            SqlMapper.AddTypeHandler(new EmbeddedDocumentConverter<List<Language>>(new LanguageIntConverter()));
            SqlMapper.AddTypeHandler(new StringListConverter<List<string>>());
            SqlMapper.AddTypeHandler(new EmbeddedDocumentConverter<ParsedEpisodeInfo>(new QualityIntConverter(), new LanguageIntConverter()));
            SqlMapper.AddTypeHandler(new EmbeddedDocumentConverter<ReleaseInfo>());
            SqlMapper.AddTypeHandler(new EmbeddedDocumentConverter<PendingReleaseAdditionalInfo>());
            SqlMapper.AddTypeHandler(new EmbeddedDocumentConverter<HashSet<int>>());
            SqlMapper.AddTypeHandler(new OsPathConverter());
            SqlMapper.RemoveTypeMap(typeof(Guid));
            SqlMapper.RemoveTypeMap(typeof(Guid?));
            SqlMapper.AddTypeHandler(new GuidConverter());
            SqlMapper.RemoveTypeMap(typeof(TimeSpan));
            SqlMapper.RemoveTypeMap(typeof(TimeSpan?));
            SqlMapper.AddTypeHandler(new TimeSpanConverter());
            SqlMapper.AddTypeHandler(new CommandConverter());
            SqlMapper.AddTypeHandler(new SystemVersionConverter());
        }

        private static void RegisterProviderSettingConverter()
        {
            var settingTypes = typeof(IProviderConfig).Assembly.ImplementationsOf<IProviderConfig>()
                .Where(x => !x.ContainsGenericParameters);

            var providerSettingConverter = new ProviderSettingConverter();
            foreach (var embeddedType in settingTypes)
            {
                SqlMapper.AddTypeHandler(embeddedType, providerSettingConverter);
            }
        }

        private static void RegisterEmbeddedConverter()
        {
            var embeddedTypes = typeof(IEmbeddedDocument).Assembly.ImplementationsOf<IEmbeddedDocument>();

            var embeddedConverterDefinition = typeof(EmbeddedDocumentConverter<>).GetGenericTypeDefinition();
            var genericListDefinition = typeof(List<>).GetGenericTypeDefinition();

            foreach (var embeddedType in embeddedTypes)
            {
                var embeddedListType = genericListDefinition.MakeGenericType(embeddedType);

                RegisterEmbeddedConverter(embeddedType, embeddedConverterDefinition);
                RegisterEmbeddedConverter(embeddedListType, embeddedConverterDefinition);
            }
        }

        private static void RegisterEmbeddedConverter(Type embeddedType, Type embeddedConverterDefinition)
        {
            var embeddedConverterType = embeddedConverterDefinition.MakeGenericType(embeddedType);
            var converter = (ITypeHandler)Activator.CreateInstance(embeddedConverterType);

            SqlMapper.AddTypeHandler(embeddedType, converter);
        }
    }
}
