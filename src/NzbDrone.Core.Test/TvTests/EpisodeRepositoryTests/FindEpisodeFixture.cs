using System.Linq;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;
using NzbDrone.Core.Test.Framework;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.Test.TvTests.EditionRepositoryTests
{
    [TestFixture]
    public class FindEpisodeFixture : DbTest<EditionRepository, Episode>
    {
        private Episode _episode1;
        private Episode _episode2;

        [SetUp]
        public void Setup()
        {
            _episode1 = Builder<Episode>.CreateNew()
                                       .With(e => e.AuthorId = 1)
                                       .With(e => e.BookNumber = 1)
                                       .With(e => e.SceneBookNumber = 2)
                                       .With(e => e.EditionNumber = 3)
                                       .With(e => e.AbsoluteEditionNumber = 3)
                                       .With(e => e.SceneEditionNumber = 4)
                                       .BuildNew();

            _episode2 = Builder<Episode>.CreateNew()
                                        .With(e => e.AuthorId = 1)
                                        .With(e => e.BookNumber = 1)
                                        .With(e => e.SceneBookNumber = 2)
                                        .With(e => e.EditionNumber = 4)
                                        .With(e => e.SceneEditionNumber = 4)
                                        .BuildNew();

            _episode1 = Db.Insert(_episode1);
        }

        [Test]
        public void should_find_episode_by_scene_numbering()
        {
            Subject.FindEpisodesBySceneNumbering(_episode1.AuthorId, _episode1.SceneBookNumber.Value, _episode1.SceneEditionNumber.Value)
                   .First()
                   .Id
                   .Should()
                   .Be(_episode1.Id);
        }

        [Test]
        public void should_find_episode_by_standard_numbering()
        {
            Subject.Find(_episode1.AuthorId, _episode1.BookNumber, _episode1.EditionNumber)
                   .Id
                   .Should()
                   .Be(_episode1.Id);
        }

        [Test]
        public void should_not_find_episode_that_does_not_exist()
        {
            Subject.Find(_episode1.AuthorId, _episode1.BookNumber + 1, _episode1.EditionNumber)
                   .Should()
                   .BeNull();
        }

        [Test]
        public void should_find_episode_by_absolute_numbering()
        {
            Subject.Find(_episode1.AuthorId, _episode1.AbsoluteEditionNumber.Value)
                .Id
                .Should()
                .Be(_episode1.Id);
        }

        [Test]
        public void should_return_multiple_episode_if_multiple_match_by_scene_numbering()
        {
            _episode2 = Db.Insert(_episode2);

            Subject.FindEpisodesBySceneNumbering(_episode1.AuthorId, _episode1.SceneBookNumber.Value, _episode1.SceneEditionNumber.Value)
                   .Should()
                   .HaveCount(2);
        }
    }
}
