﻿using System;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;
using Readarr.Core.Download.Pending;
using Readarr.Core.Housekeeping.Housekeepers;
using Readarr.Core.Parser.Model;
using Readarr.Core.Test.Framework;
using Readarr.Core.Tv;

namespace Readarr.Core.Test.Housekeeping.Housekeepers
{
    [TestFixture]
    public class CleanupDownloadClientUnavailablePendingReleasesFixture : DbTest<CleanupDownloadClientUnavailablePendingReleases, PendingRelease>
    {
        [Test]
        public void should_delete_old_DownloadClientUnavailable_pending_items()
        {
            var pendingRelease = Builder<PendingRelease>.CreateNew()
                .With(h => h.Reason = PendingReleaseReason.DownloadClientUnavailable)
                .With(h => h.Added = DateTime.UtcNow.AddDays(-21))
                .With(h => h.ParsedEpisodeInfo = new ParsedEpisodeInfo())
                .With(h => h.Release = new ReleaseInfo())
                .BuildNew();

            Db.Insert(pendingRelease);
            Subject.Clean();
            AllStoredModels.Should().BeEmpty();
        }

        [Test]
        public void should_delete_old_Fallback_pending_items()
        {
            var pendingRelease = Builder<PendingRelease>.CreateNew()
                .With(h => h.Reason = PendingReleaseReason.Fallback)
                .With(h => h.Added = DateTime.UtcNow.AddDays(-21))
                .With(h => h.ParsedEpisodeInfo = new ParsedEpisodeInfo())
                .With(h => h.Release = new ReleaseInfo())
                .BuildNew();

            Db.Insert(pendingRelease);
            Subject.Clean();
            AllStoredModels.Should().BeEmpty();
        }

        [Test]
        public void should_not_delete_old_Delay_pending_items()
        {
            var pendingRelease = Builder<PendingRelease>.CreateNew()
                .With(h => h.Reason = PendingReleaseReason.Delay)
                .With(h => h.Added = DateTime.UtcNow.AddDays(-21))
                .With(h => h.ParsedEpisodeInfo = new ParsedEpisodeInfo())
                .With(h => h.Release = new ReleaseInfo())
                .BuildNew();

            Db.Insert(pendingRelease);
            Subject.Clean();
            AllStoredModels.Should().HaveCount(1);
        }
    }
}
