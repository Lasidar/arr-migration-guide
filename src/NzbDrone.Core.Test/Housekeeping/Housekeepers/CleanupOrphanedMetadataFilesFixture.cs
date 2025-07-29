using System.Collections.Generic;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;
using NzbDrone.Core.Extras.Metadata;
using NzbDrone.Core.Extras.Metadata.Files;
using NzbDrone.Core.Housekeeping.Housekeepers;
using NzbDrone.Core.Languages;
using NzbDrone.Core.MediaFiles;
using NzbDrone.Core.Qualities;
using NzbDrone.Core.Test.Framework;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.Test.Housekeeping.Housekeepers
{
    [TestFixture]
    public class CleanupOrphanedMetadataFilesFixture : DbTest<CleanupOrphanedMetadataFiles, MetadataFile>
    {
        [Test]
        public void should_delete_metadata_files_that_dont_have_a_coresponding_series()
        {
            var metadataFile = Builder<MetadataFile>.CreateNew()
                                                    .With(m => m.EditionFileId = null)
                                                    .BuildNew();

            Db.Insert(metadataFile);
            Subject.Clean();
            AllStoredModels.Should().BeEmpty();
        }

        [Test]
        public void should_not_delete_metadata_files_that_have_a_coresponding_series()
        {
            var series = Builder<Series>.CreateNew()
                                        .BuildNew();

            Db.Insert(series);

            var metadataFile = Builder<MetadataFile>.CreateNew()
                                                    .With(m => m.AuthorId = series.Id)
                                                    .With(m => m.EditionFileId = null)
                                                    .BuildNew();

            Db.Insert(metadataFile);
            Subject.Clean();
            AllStoredModels.Should().HaveCount(1);
        }

        [Test]
        public void should_delete_metadata_files_that_dont_have_a_coresponding_episode_file()
        {
            var series = Builder<Series>.CreateNew()
                                        .BuildNew();

            Db.Insert(series);

            var metadataFile = Builder<MetadataFile>.CreateNew()
                                                    .With(m => m.AuthorId = series.Id)
                                                    .With(m => m.EditionFileId = 10)
                                                    .BuildNew();

            Db.Insert(metadataFile);
            Subject.Clean();
            AllStoredModels.Should().BeEmpty();
        }

        [Test]
        public void should_not_delete_metadata_files_that_have_a_coresponding_episode_file()
        {
            var series = Builder<Series>.CreateNew()
                                        .BuildNew();

            var episodeFile = Builder<EditionFile>.CreateNew()
                .With(h => h.Quality = new QualityModel())
                .With(h => h.Languages = new List<Language> { Language.English })
                .BuildNew();

            Db.Insert(series);
            Db.Insert(episodeFile);

            var metadataFile = Builder<MetadataFile>.CreateNew()
                                                    .With(m => m.AuthorId = series.Id)
                                                    .With(m => m.EditionFileId = episodeFile.Id)
                                                    .BuildNew();

            Db.Insert(metadataFile);
            Subject.Clean();
            AllStoredModels.Should().HaveCount(1);
        }

        [Test]
        public void should_delete_episode_metadata_files_that_have_episodefileid_of_zero()
        {
            var series = Builder<Series>.CreateNew()
                                        .BuildNew();

            Db.Insert(series);

            var metadataFile = Builder<MetadataFile>.CreateNew()
                                                 .With(m => m.AuthorId = series.Id)
                                                 .With(m => m.Type = MetadataType.EpisodeMetadata)
                                                 .With(m => m.EditionFileId = 0)
                                                 .BuildNew();

            Db.Insert(metadataFile);
            Subject.Clean();
            AllStoredModels.Should().HaveCount(0);
        }

        [Test]
        public void should_delete_episode_image_files_that_have_episodefileid_of_zero()
        {
            var series = Builder<Series>.CreateNew()
                                        .BuildNew();

            Db.Insert(series);

            var metadataFile = Builder<MetadataFile>.CreateNew()
                                                    .With(m => m.AuthorId = series.Id)
                                                    .With(m => m.Type = MetadataType.EpisodeImage)
                                                    .With(m => m.EditionFileId = 0)
                                                    .BuildNew();

            Db.Insert(metadataFile);
            Subject.Clean();
            AllStoredModels.Should().HaveCount(0);
        }
    }
}
