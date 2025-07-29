using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using Moq;
using NUnit.Framework;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.Configuration;
using NzbDrone.Core.Datastore;
using NzbDrone.Core.MediaFiles;
using NzbDrone.Core.MediaFiles.Events;
using NzbDrone.Core.Test.Framework;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.Test.TvTests.EditionServiceTests
{
    [TestFixture]
    public class HandleEditionFileDeletedFixture : CoreTest<EditionService>
    {
        private Series _series;
        private EditionFile _episodeFile;
        private List<Episode> _episodes;

        [SetUp]
        public void Setup()
        {
            _series = Builder<Series>
                .CreateNew()
                .Build();

            _episodeFile = Builder<EditionFile>
                .CreateNew()
                .With(e => e.AuthorId = _series.Id)
                .Build();
        }

        private void GivenSingleEditionFile()
        {
            _episodes = Builder<Episode>
                .CreateListOfSize(1)
                .All()
                .With(e => e.AuthorId = _series.Id)
                .With(e => e.Monitored = true)
                .Build()
                .ToList();

            Mocker.GetMock<IEditionRepository>()
                  .Setup(s => s.GetEpisodeByFileId(_episodeFile.Id))
                  .Returns(_episodes);
        }

        private void GivenMultiEditionFile()
        {
            _episodes = Builder<Episode>
                .CreateListOfSize(2)
                .All()
                .With(e => e.AuthorId = _series.Id)
                .With(e => e.Monitored = true)
                .Build()
                .ToList();

            Mocker.GetMock<IEditionRepository>()
                  .Setup(s => s.GetEpisodeByFileId(_episodeFile.Id))
                  .Returns(_episodes);
        }

        [Test]
        public void should_set_EditionFileId_to_zero()
        {
            GivenSingleEditionFile();

            Subject.Handle(new EditionFileDeletedEvent(_episodeFile, DeleteMediaFileReason.MissingFromDisk));

            Mocker.GetMock<IEditionRepository>()
                .Verify(v => v.ClearFileId(It.IsAny<Episode>(), It.IsAny<bool>()), Times.Once());
        }

        [Test]
        public void should_update_each_episode_for_file()
        {
            GivenMultiEditionFile();

            Subject.Handle(new EditionFileDeletedEvent(_episodeFile, DeleteMediaFileReason.MissingFromDisk));

            Mocker.GetMock<IEditionRepository>()
                .Verify(v => v.ClearFileId(It.IsAny<Episode>(), It.IsAny<bool>()), Times.Exactly(2));
        }

        [Test]
        public void should_set_monitored_to_false_if_autoUnmonitor_is_true_and_is_not_for_an_upgrade()
        {
            GivenSingleEditionFile();

            Mocker.GetMock<IConfigService>()
                  .SetupGet(s => s.AutoUnmonitorPreviouslyDownloadedEpisodes)
                  .Returns(true);

            Subject.Handle(new EditionFileDeletedEvent(_episodeFile, DeleteMediaFileReason.MissingFromDisk));
            Subject.HandleAsync(new SeriesScannedEvent(_series, new List<string>()));

            Mocker.GetMock<IEditionRepository>()
                .Verify(v => v.SetMonitored(It.IsAny<IEnumerable<int>>(), false), Times.Once());
        }

        [Test]
        public void should_leave_monitored_if_autoUnmonitor_is_true_and_missing_episode_is_replaced()
        {
            GivenSingleEditionFile();

            var newEditionFile = _episodeFile.JsonClone();
            newEditionFile.Id = 123;
            newEditionFile.Episodes = new LazyLoaded<List<Episode>>(_episodes);

            Mocker.GetMock<IConfigService>()
                .SetupGet(s => s.AutoUnmonitorPreviouslyDownloadedEpisodes)
                .Returns(true);

            Subject.Handle(new EditionFileDeletedEvent(_episodeFile, DeleteMediaFileReason.MissingFromDisk));
            Subject.Handle(new EditionFileAddedEvent(newEditionFile));
            Subject.HandleAsync(new SeriesScannedEvent(_series, new List<string>()));

            Mocker.GetMock<IEditionRepository>()
                .Verify(v => v.SetMonitored(It.IsAny<IEnumerable<int>>(), false), Times.Never());
        }

        [Test]
        public void should_leave_monitored_to_true_if_autoUnmonitor_is_false()
        {
            GivenSingleEditionFile();

            Mocker.GetMock<IConfigService>()
                  .SetupGet(s => s.AutoUnmonitorPreviouslyDownloadedEpisodes)
                  .Returns(false);

            Subject.Handle(new EditionFileDeletedEvent(_episodeFile, DeleteMediaFileReason.Upgrade));

            Mocker.GetMock<IEditionRepository>()
                .Verify(v => v.ClearFileId(It.IsAny<Episode>(), false), Times.Once());
        }

        [Test]
        public void should_leave_monitored_to_true_if_autoUnmonitor_is_true_and_is_for_an_upgrade()
        {
            GivenSingleEditionFile();

            Mocker.GetMock<IConfigService>()
                  .SetupGet(s => s.AutoUnmonitorPreviouslyDownloadedEpisodes)
                  .Returns(true);

            Subject.Handle(new EditionFileDeletedEvent(_episodeFile, DeleteMediaFileReason.Upgrade));

            Mocker.GetMock<IEditionRepository>()
                .Verify(v => v.ClearFileId(It.IsAny<Episode>(), false), Times.Once());
        }

        [Test]
        public void should_leave_monitored_to_true_if_autoUnmonitor_is_true_and_is_for_manual_override()
        {
            GivenSingleEditionFile();

            Mocker.GetMock<IConfigService>()
                  .SetupGet(s => s.AutoUnmonitorPreviouslyDownloadedEpisodes)
                  .Returns(true);

            Subject.Handle(new EditionFileDeletedEvent(_episodeFile, DeleteMediaFileReason.ManualOverride));

            Mocker.GetMock<IEditionRepository>()
                  .Verify(v => v.ClearFileId(It.IsAny<Episode>(), false), Times.Once());
        }
    }
}
