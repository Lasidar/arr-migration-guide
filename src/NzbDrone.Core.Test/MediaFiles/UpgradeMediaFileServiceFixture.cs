using System.IO;
using System.Linq;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using NzbDrone.Common.Disk;
using NzbDrone.Core.Datastore;
using NzbDrone.Core.MediaFiles;
using NzbDrone.Core.MediaFiles.EpisodeImport;
using NzbDrone.Core.Parser.Model;
using NzbDrone.Core.Test.Framework;
using NzbDrone.Core.Books;
using NzbDrone.Test.Common;

namespace NzbDrone.Core.Test.MediaFiles
{
    public class UpgradeMediaFileServiceFixture : CoreTest<UpgradeMediaFileService>
    {
        private EditionFile _episodeFile;
        private LocalEpisode _localEpisode;

        [SetUp]
        public void Setup()
        {
            _localEpisode = new LocalEpisode();
            _localEpisode.Series = new Series
                                   {
                                       Path = @"C:\Test\TV\Series".AsOsAgnostic()
                                   };

            _episodeFile = Builder<EditionFile>
                .CreateNew()
                .Build();

            Mocker.GetMock<IDiskProvider>()
                  .Setup(c => c.FolderExists(Directory.GetParent(_localEpisode.Series.Path).FullName))
                  .Returns(true);

            Mocker.GetMock<IDiskProvider>()
                  .Setup(c => c.FileExists(It.IsAny<string>()))
                  .Returns(true);

            Mocker.GetMock<IDiskProvider>()
                  .Setup(c => c.GetParentFolder(It.IsAny<string>()))
                  .Returns<string>(c => Path.GetDirectoryName(c));
        }

        private void GivenSingleEpisodeWithSingleEditionFile()
        {
            _localEpisode.Episodes = Builder<Episode>.CreateListOfSize(1)
                                                     .All()
                                                     .With(e => e.EditionFileId = 1)
                                                     .With(e => e.EditionFile = new EditionFile
                                                                                {
                                                                                    Id = 1,
                                                                                    RelativePath = @"Season 01\30.rock.s01e01.avi",
                                                                                })
                                                     .Build()
                                                     .ToList();
        }

        private void GivenMultipleEpisodesWithSingleEditionFile()
        {
            _localEpisode.Episodes = Builder<Episode>.CreateListOfSize(2)
                                                     .All()
                                                     .With(e => e.EditionFileId = 1)
                                                     .With(e => e.EditionFile = new EditionFile
                                                                                {
                                                                                    Id = 1,
                                                                                    RelativePath = @"Season 01\30.rock.s01e01.avi",
                                                                                })
                                                     .Build()
                                                     .ToList();
        }

        private void GivenMultipleEpisodesWithMultipleEditionFiles()
        {
            _localEpisode.Episodes = Builder<Episode>.CreateListOfSize(2)
                                                     .TheFirst(1)
                                                     .With(e => e.EditionFile = new EditionFile
                                                                                {
                                                                                    Id = 1,
                                                                                    RelativePath = @"Season 01\30.rock.s01e01.avi",
                                                                                })
                                                     .TheNext(1)
                                                     .With(e => e.EditionFile = new EditionFile
                                                                                {
                                                                                    Id = 2,
                                                                                    RelativePath = @"Season 01\30.rock.s01e02.avi",
                                                                                })
                                                     .Build()
                                                     .ToList();
        }

        [Test]
        public void should_delete_single_episode_file_once()
        {
            GivenSingleEpisodeWithSingleEditionFile();

            Subject.UpgradeEditionFile(_episodeFile, _localEpisode);

            Mocker.GetMock<IRecycleBinProvider>().Verify(v => v.DeleteFile(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [Test]
        public void should_delete_the_same_episode_file_only_once()
        {
            GivenMultipleEpisodesWithSingleEditionFile();

            Subject.UpgradeEditionFile(_episodeFile, _localEpisode);

            Mocker.GetMock<IRecycleBinProvider>().Verify(v => v.DeleteFile(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [Test]
        public void should_delete_multiple_different_episode_files()
        {
            GivenMultipleEpisodesWithMultipleEditionFiles();

            Subject.UpgradeEditionFile(_episodeFile, _localEpisode);

            Mocker.GetMock<IRecycleBinProvider>().Verify(v => v.DeleteFile(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        }

        [Test]
        public void should_delete_episode_file_from_database()
        {
            GivenSingleEpisodeWithSingleEditionFile();

            Subject.UpgradeEditionFile(_episodeFile, _localEpisode);

            Mocker.GetMock<IMediaFileService>().Verify(v => v.Delete(It.IsAny<EditionFile>(), DeleteMediaFileReason.Upgrade), Times.Once());
        }

        [Test]
        public void should_delete_existing_file_fromdb_if_file_doesnt_exist()
        {
            GivenSingleEpisodeWithSingleEditionFile();

            Mocker.GetMock<IDiskProvider>()
                .Setup(c => c.FileExists(It.IsAny<string>()))
                .Returns(false);

            Subject.UpgradeEditionFile(_episodeFile, _localEpisode);

            Mocker.GetMock<IMediaFileService>().Verify(v => v.Delete(_localEpisode.Episodes.Single().EditionFile, DeleteMediaFileReason.Upgrade), Times.Once());
        }

        [Test]
        public void should_not_try_to_recyclebin_existing_file_if_file_doesnt_exist()
        {
            GivenSingleEpisodeWithSingleEditionFile();

            Mocker.GetMock<IDiskProvider>()
                .Setup(c => c.FileExists(It.IsAny<string>()))
                .Returns(false);

            Subject.UpgradeEditionFile(_episodeFile, _localEpisode);

            Mocker.GetMock<IRecycleBinProvider>().Verify(v => v.DeleteFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void should_return_old_episode_file_in_oldFiles()
        {
            GivenSingleEpisodeWithSingleEditionFile();

            Subject.UpgradeEditionFile(_episodeFile, _localEpisode).OldFiles.Count.Should().Be(1);
        }

        [Test]
        public void should_return_old_episode_files_in_oldFiles()
        {
            GivenMultipleEpisodesWithMultipleEditionFiles();

            Subject.UpgradeEditionFile(_episodeFile, _localEpisode).OldFiles.Count.Should().Be(2);
        }

        [Test]
        public void should_throw_if_there_are_existing_episode_files_and_the_root_folder_is_missing()
        {
            GivenSingleEpisodeWithSingleEditionFile();

            Mocker.GetMock<IDiskProvider>()
                  .Setup(c => c.FolderExists(Directory.GetParent(_localEpisode.Series.Path).FullName))
                  .Returns(false);

            Assert.Throws<RootFolderNotFoundException>(() => Subject.UpgradeEditionFile(_episodeFile, _localEpisode));

            Mocker.GetMock<IMediaFileService>().Verify(v => v.Delete(_localEpisode.Episodes.Single().EditionFile, DeleteMediaFileReason.Upgrade), Times.Never());
        }

        [Test]
        public void should_import_if_existing_file_doesnt_exist_in_db()
        {
            _localEpisode.Episodes = Builder<Episode>.CreateListOfSize(1)
                                                     .All()
                                                     .With(e => e.EditionFileId = 1)
                                                     .With(e => e.EditionFile = new LazyLoaded<EditionFile>(null))
                                                     .Build()
                                                     .ToList();

            Subject.UpgradeEditionFile(_episodeFile, _localEpisode);

            Mocker.GetMock<IMediaFileService>().Verify(v => v.Delete(_localEpisode.Episodes.Single().EditionFile, It.IsAny<DeleteMediaFileReason>()), Times.Never());
        }
    }
}
