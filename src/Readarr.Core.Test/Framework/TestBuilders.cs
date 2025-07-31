using System;
using System.Collections.Generic;
using FizzWare.NBuilder;
using Readarr.Core.Books;
using Readarr.Core.Datastore;
using Readarr.Core.MediaFiles;
using Readarr.Core.Profiles.Qualities;
using Readarr.Core.Qualities;

namespace Readarr.Core.Test.Framework
{
    public static class TestBuilders
    {
        public static Author CreateAuthor(int id = 0, string name = null)
        {
            var author = Builder<Author>.CreateNew()
                .With(a => a.Id = id > 0 ? id : 0)
                .With(a => a.Name = name ?? "Test Author")
                .With(a => a.CleanName = (name ?? "Test Author").ToLowerInvariant())
                .With(a => a.Path = $@"C:\Test\Authors\{name ?? "Test Author"}")
                .With(a => a.Monitored = true)
                .With(a => a.QualityProfile = new LazyLoaded<QualityProfile>(CreateQualityProfile()))
                .With(a => a.Metadata = new LazyLoaded<AuthorMetadata>(CreateAuthorMetadata()))
                .Build();

            return author;
        }

        public static AuthorMetadata CreateAuthorMetadata(string name = null)
        {
            return Builder<AuthorMetadata>.CreateNew()
                .With(m => m.Name = name ?? "Test Author")
                .With(m => m.Status = AuthorStatusType.Continuing)
                .With(m => m.GoodreadsId = "12345")
                .Build();
        }

        public static Book CreateBook(int id = 0, string title = null, Author author = null)
        {
            var book = Builder<Book>.CreateNew()
                .With(b => b.Id = id > 0 ? id : 0)
                .With(b => b.Title = title ?? "Test Book")
                .With(b => b.CleanTitle = (title ?? "Test Book").ToLowerInvariant())
                .With(b => b.AuthorId = author?.Id ?? 1)
                .With(b => b.Author = new LazyLoaded<Author>(author ?? CreateAuthor()))
                .With(b => b.Monitored = true)
                .With(b => b.BookFiles = new LazyLoaded<List<BookFile>>(new List<BookFile>()))
                .Build();

            return book;
        }

        public static BookFile CreateBookFile(int id = 0, Book book = null, QualityModel quality = null)
        {
            var bookFile = Builder<BookFile>.CreateNew()
                .With(f => f.Id = id > 0 ? id : 0)
                .With(f => f.Path = @"C:\Test\book.epub")
                .With(f => f.Size = 1024 * 1024) // 1MB
                .With(f => f.DateAdded = DateTime.UtcNow)
                .With(f => f.Quality = quality ?? new QualityModel(Quality.EPUB))
                .With(f => f.ReleaseGroup = "TestGroup")
                .Build();

            if (book != null)
            {
                bookFile.BookId = book.Id;
                bookFile.Books = new LazyLoaded<List<Book>>(new List<Book> { book });
            }

            return bookFile;
        }

        public static QualityProfile CreateQualityProfile(string name = null)
        {
            var profile = new QualityProfile
            {
                Name = name ?? "Test Profile",
                Cutoff = Quality.EPUB.Id,
                Items = new List<QualityProfileQualityItem>
                {
                    new QualityProfileQualityItem { Quality = Quality.PDF, Allowed = true },
                    new QualityProfileQualityItem { Quality = Quality.MOBI, Allowed = true },
                    new QualityProfileQualityItem { Quality = Quality.EPUB, Allowed = true },
                    new QualityProfileQualityItem { Quality = Quality.AZW3, Allowed = true }
                }
            };

            return profile;
        }
    }
}