using System.Collections.Generic;
using System.IO;
using System.Linq;
using FizzWare.NBuilder;
using Moq;
using NUnit.Framework;
using NzbDrone.Common.Disk;
using NzbDrone.Core.MediaFiles;
using NzbDrone.Core.Test.Framework;
using NzbDrone.Core.Books;
using NzbDrone.Test.Common;

namespace NzbDrone.Core.Test.MediaFiles
{
    public class MediaFileTableCleanupServiceFixture : CoreTest<MediaFileTableCleanupService>
    {
        private const string DELETED_PATH = "ANY FILE WITH THIS PATH IS CONSIDERED DELETED!";
        private List<Episode> _episodes;
        private Series _series;

        [SetUp]
        public void SetUp()
        {
            _episodes = Builder<Episode>.CreateListOfSize(10)
                  .Build()
                  .ToList();

            _series = Builder<Series>.CreateNew()
                                     .With(s => s.Path = @"C:\Test\TV\Series".AsOsAgnostic())
                                     .Build();

            Mocker.GetMock<IDiskProvider>()
                  .Setup(e => e.FileExists(It.Is<string>(c => !c.Contains(DELETED_PATH))))
                  .Returns(true);

            Mocker.GetMock<IEditionService>()
                  .Setup(c => c.GetEpisodeBySeries(It.IsAny<int>()))
                  .Returns(_episodes);
        }

        private void GivenEditionFiles(IEnumerable<EditionFile> episodeFiles)
        {
            Mocker.GetMock<IMediaFileService>()
                  .Setup(c => c.GetFilesBySeries(It.IsAny<int>()))
                  .Returns(episodeFiles.ToList());
        }

        private void GivenFilesAreNotAttachedToEpisode()
        {
            _episodes.ForEach(e => e.EditionFileId = 0);

            Mocker.GetMock<IEditionService>()
                  .Setup(c => c.GetEpisodeBySeries(It.IsAny<int>()))
                  .Returns(_episodes);
        }

        private List<string> FilesOnDisk(IEnumerable<EditionFile> episodeFiles)
        {
            return episodeFiles.Select(e => Path.Combine(_series.Path, e.RelativePath)).ToList();
        }

        [Test]
        public void should_skip_files_that_exist_in_disk()
        {
            var episodeFiles = Builder<EditionFile>.CreateListOfSize(10)
                .Build();

            GivenEditionFiles(episodeFiles);

            Subject.Clean(_series, FilesOnDisk(episodeFiles));

            Mocker.GetMock<IEditionService>().Verify(c => c.UpdateEpisode(It.IsAny<Episode>()), Times.Never());
        }

        [Test]
        public void should_delete_non_existent_files()
        {
            var episodeFiles = Builder<EditionFile>.CreateListOfSize(10)
                .Random(2)
                .With(c => c.RelativePath = DELETED_PATH)
                .Build();

            GivenEditionFiles(episodeFiles);

            Subject.Clean(_series, FilesOnDisk(episodeFiles.Where(e => e.RelativePath != DELETED_PATH)));

            Mocker.GetMock<IMediaFileService>().Verify(c => c.Delete(It.Is<EditionFile>(e => e.RelativePath == DELETED_PATH), DeleteMediaFileReason.MissingFromDisk), Times.Exactly(2));
        }

        [Test]
        public void should_delete_files_that_dont_belong_to_any_episodes()
        {
            var episodeFiles = Builder<EditionFile>.CreateListOfSize(10)
                                .Random(10)
                                .With(c => c.RelativePath = "ExistingPath")
                                .Build();

            GivenEditionFiles(episodeFiles);
            GivenFilesAreNotAttachedToEpisode();

            Subject.Clean(_series, FilesOnDisk(episodeFiles));

            Mocker.GetMock<IMediaFileService>().Verify(c => c.Delete(It.IsAny<EditionFile>(), DeleteMediaFileReason.NoLinkedEpisodes), Times.Exactly(10));
        }

        [Test]
        public void should_unlink_episode_when_episodeFile_does_not_exist()
        {
            GivenEditionFiles(new List<EditionFile>());

            Subject.Clean(_series, new List<string>());

            Mocker.GetMock<IEditionService>().Verify(c => c.UpdateEpisode(It.Is<Episode>(e => e.EditionFileId == 0)), Times.Exactly(10));
        }

        [Test]
        public void should_not_update_episode_when_episodeFile_exists()
        {
            var episodeFiles = Builder<EditionFile>.CreateListOfSize(10)
                                .Random(10)
                                .With(c => c.RelativePath = "ExistingPath")
                                .Build();

            GivenEditionFiles(episodeFiles);

            Subject.Clean(_series, FilesOnDisk(episodeFiles));

            Mocker.GetMock<IEditionService>().Verify(c => c.UpdateEpisode(It.IsAny<Episode>()), Times.Never());
        }
    }
}
