﻿using System.Collections.Generic;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Readarr.Common.Disk;
using Readarr.Core.Download;
using Readarr.Core.Download.TrackedDownloads;
using Readarr.Core.History;
using Readarr.Core.Messaging.Events;
using Readarr.Core.Parser.Model;
using Readarr.Core.Test.Framework;
using Readarr.Core.Books;
using Readarr.Test.Common;

namespace Readarr.Core.Test.Download.FailedDownloadServiceTests
{
    [TestFixture]
    public class ProcessFailedFixture : CoreTest<FailedDownloadService>
    {
        private TrackedDownload _trackedDownload;
        private List<EpisodeHistory> _grabHistory;

        [SetUp]
        public void Setup()
        {
            var completed = Builder<DownloadClientItem>.CreateNew()
                                                    .With(h => h.Status = DownloadItemStatus.Completed)
                                                    .With(h => h.OutputPath = new OsPath(@"C:\DropFolder\MyDownload".AsOsAgnostic()))
                                                    .With(h => h.Title = "Drone.S01E01.HDTV")
                                                    .Build();

            _grabHistory = Builder<EpisodeHistory>.CreateListOfSize(2).BuildList();

            var remoteEpisode = new RemoteEpisode
            {
                Series = new Series(),
                Episodes = new List<Episode> { new Episode { Id = 1 } }
            };

            _trackedDownload = Builder<TrackedDownload>.CreateNew()
                    .With(c => c.State = TrackedDownloadState.FailedPending)
                    .With(c => c.DownloadItem = completed)
                    .With(c => c.RemoteEpisode = remoteEpisode)
                    .Build();

            Mocker.GetMock<IHistoryService>()
                  .Setup(s => s.Find(_trackedDownload.DownloadItem.DownloadId, EpisodeHistoryEventType.Grabbed))
                  .Returns(_grabHistory);
        }

        [Test]
        public void should_mark_failed_if_encrypted()
        {
            _trackedDownload.DownloadItem.IsEncrypted = true;

            Subject.ProcessFailed(_trackedDownload);

            AssertDownloadFailed();
        }

        [Test]
        public void should_mark_failed_if_download_item_is_failed()
        {
            _trackedDownload.DownloadItem.Status = DownloadItemStatus.Failed;

            Subject.ProcessFailed(_trackedDownload);

            AssertDownloadFailed();
        }

        [Test]
        public void should_include_tracked_download_in_message()
        {
            _trackedDownload.DownloadItem.Status = DownloadItemStatus.Failed;

            Subject.ProcessFailed(_trackedDownload);

            Mocker.GetMock<IEventAggregator>()
                  .Verify(v => v.PublishEvent(It.Is<DownloadFailedEvent>(c => c.TrackedDownload != null)), Times.Once());

            AssertDownloadFailed();
        }

        private void AssertDownloadNotFailed()
        {
            Mocker.GetMock<IEventAggregator>()
               .Verify(v => v.PublishEvent(It.IsAny<DownloadFailedEvent>()), Times.Never());

            _trackedDownload.State.Should().NotBe(TrackedDownloadState.Failed);
        }

        private void AssertDownloadFailed()
        {
            Mocker.GetMock<IEventAggregator>()
            .Verify(v => v.PublishEvent(It.IsAny<DownloadFailedEvent>()), Times.Once());

            _trackedDownload.State.Should().Be(TrackedDownloadState.Failed);
        }
    }
}
