using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using Moq;
using NUnit.Framework;
using NzbDrone.Core.MediaFiles;
using NzbDrone.Core.MediaFiles.Commands;
using NzbDrone.Core.MediaFiles.Events;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.Core.Test.Framework;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.Test.MediaFiles
{
    public class RenameEditionFileServiceFixture : CoreTest<RenameEditionFileService>
    {
        private Series _series;
        private List<EditionFile> _episodeFiles;

        [SetUp]
        public void Setup()
        {
            _series = Builder<Series>.CreateNew()
                                     .Build();

            _episodeFiles = Builder<EditionFile>.CreateListOfSize(2)
                                                .All()
                                                .With(e => e.AuthorId = _series.Id)
                                                .With(e => e.BookNumber = 1)
                                                .Build()
                                                .ToList();

            Mocker.GetMock<IAuthorService>()
                  .Setup(s => s.GetSeries(_series.Id))
                  .Returns(_series);
        }

        private void GivenNoEditionFiles()
        {
            Mocker.GetMock<IMediaFileService>()
                  .Setup(s => s.Get(It.IsAny<IEnumerable<int>>()))
                  .Returns(new List<EditionFile>());
        }

        private void GivenEditionFiles()
        {
            Mocker.GetMock<IMediaFileService>()
                  .Setup(s => s.Get(It.IsAny<IEnumerable<int>>()))
                  .Returns(_episodeFiles);
        }

        private void GivenMovedFiles()
        {
            Mocker.GetMock<IMoveEditionFiles>()
                  .Setup(s => s.MoveEditionFile(It.IsAny<EditionFile>(), _series));
        }

        [Test]
        public void should_not_publish_event_if_no_files_to_rename()
        {
            GivenNoEditionFiles();

            Subject.Execute(new RenameFilesCommand(_series.Id, new List<int> { 1 }));

            Mocker.GetMock<IEventAggregator>()
                  .Verify(v => v.PublishEvent(It.IsAny<SeriesRenamedEvent>()), Times.Never());
        }

        [Test]
        public void should_not_publish_event_if_no_files_are_renamed()
        {
            GivenEditionFiles();

            Mocker.GetMock<IMoveEditionFiles>()
                  .Setup(s => s.MoveEditionFile(It.IsAny<EditionFile>(), It.IsAny<Series>()))
                  .Throws(new SameFilenameException("Same file name", "Filename"));

            Subject.Execute(new RenameFilesCommand(_series.Id, new List<int> { 1 }));

            Mocker.GetMock<IEventAggregator>()
                  .Verify(v => v.PublishEvent(It.IsAny<SeriesRenamedEvent>()), Times.Never());
        }

        [Test]
        public void should_publish_event_if_files_are_renamed()
        {
            GivenEditionFiles();
            GivenMovedFiles();

            Subject.Execute(new RenameFilesCommand(_series.Id, new List<int> { 1 }));

            Mocker.GetMock<IEventAggregator>()
                  .Verify(v => v.PublishEvent(It.IsAny<SeriesRenamedEvent>()), Times.Once());
        }

        [Test]
        public void should_update_moved_files()
        {
            GivenEditionFiles();
            GivenMovedFiles();

            Subject.Execute(new RenameFilesCommand(_series.Id, new List<int> { 1 }));

            Mocker.GetMock<IMediaFileService>()
                  .Verify(v => v.Update(It.IsAny<EditionFile>()), Times.Exactly(2));
        }

        [Test]
        public void should_get_episodefiles_by_ids_only()
        {
            GivenEditionFiles();
            GivenMovedFiles();

            var files = new List<int> { 1 };

            Subject.Execute(new RenameFilesCommand(_series.Id, files));

            Mocker.GetMock<IMediaFileService>()
                  .Verify(v => v.Get(files), Times.Once());
        }
    }
}
