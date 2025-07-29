using System.Collections.Generic;
using System.Linq;
using NLog;
using Readarr.Common.Extensions;
using Readarr.Core.AutoTagging;
using Readarr.Core.Books.Events;
using Readarr.Core.Messaging.Events;
using Readarr.Core.Parser;

namespace Readarr.Core.Books
{
    public interface IAuthorService
    {
        Author GetAuthor(int authorId);
        Author GetAuthorByMetadataId(int authorMetadataId);
        List<Author> GetAuthors(IEnumerable<int> authorIds);
        Author AddAuthor(Author newAuthor);
        List<Author> AddAuthors(List<Author> newAuthors);
        Author FindByForeignAuthorId(string foreignAuthorId);
        Author FindByGoodreadsId(string goodreadsId);
        Author FindByName(string name);
        List<Author> FindByNameInexact(string name);
        Author FindByPath(string path);
        void DeleteAuthor(List<int> authorIds, bool deleteFiles, bool addImportListExclusion);
        List<Author> GetAllAuthors();
        List<string> AllAuthorForeignIds();
        Dictionary<int, string> GetAllAuthorPaths();
        Dictionary<int, List<int>> GetAllAuthorTags();
        List<Author> AllForTag(int tagId);
        Author UpdateAuthor(Author author, bool publishUpdatedEvent = true);
        List<Author> UpdateAuthors(List<Author> authors, bool useExistingRelativeFolder);
        bool AuthorPathExists(string folder);
        void RemoveAddOptions(Author author);
        bool UpdateTags(Author author);
    }

    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IAuthorMetadataRepository _authorMetadataRepository;
        private readonly IEventAggregator _eventAggregator;
        private readonly IBookService _bookService;
        private readonly IBuildAuthorPaths _authorPathBuilder;
        private readonly IAutoTaggingService _autoTaggingService;
        private readonly Logger _logger;

        public AuthorService(IAuthorRepository authorRepository,
                            IAuthorMetadataRepository authorMetadataRepository,
                            IEventAggregator eventAggregator,
                            IBookService bookService,
                            IBuildAuthorPaths authorPathBuilder,
                            IAutoTaggingService autoTaggingService,
                            Logger logger)
        {
            _authorRepository = authorRepository;
            _authorMetadataRepository = authorMetadataRepository;
            _eventAggregator = eventAggregator;
            _bookService = bookService;
            _authorPathBuilder = authorPathBuilder;
            _autoTaggingService = autoTaggingService;
            _logger = logger;
        }

        public Author GetAuthor(int authorId)
        {
            return _authorRepository.Get(authorId);
        }

        public Author GetAuthorByMetadataId(int authorMetadataId)
        {
            return _authorRepository.GetAuthorByMetadataId(authorMetadataId);
        }

        public List<Author> GetAuthors(IEnumerable<int> authorIds)
        {
            return _authorRepository.Get(authorIds).ToList();
        }

        public Author AddAuthor(Author newAuthor)
        {
            _authorRepository.Insert(newAuthor);
            _eventAggregator.PublishEvent(new AuthorAddedEvent(GetAuthor(newAuthor.Id)));

            return newAuthor;
        }

        public List<Author> AddAuthors(List<Author> newAuthors)
        {
            _authorRepository.InsertMany(newAuthors);
            _eventAggregator.PublishEvent(new AuthorsImportedEvent(newAuthors.Select(a => a.Id).ToList()));

            return newAuthors;
        }

        public Author FindByForeignAuthorId(string foreignAuthorId)
        {
            return _authorRepository.FindByForeignAuthorId(foreignAuthorId);
        }

        public Author FindByGoodreadsId(string goodreadsId)
        {
            return _authorRepository.FindByGoodreadsId(goodreadsId);
        }

        public Author FindByName(string name)
        {
            return _authorRepository.FindByName(name);
        }

        public List<Author> FindByNameInexact(string name)
        {
            return _authorRepository.FindByNameInexact(name);
        }

        public Author FindByPath(string path)
        {
            return _authorRepository.FindByPath(path);
        }

        public void DeleteAuthor(List<int> authorIds, bool deleteFiles, bool addImportListExclusion)
        {
            var authorsToDelete = _authorRepository.Get(authorIds).ToList();

            _authorRepository.DeleteMany(authorIds);

            authorsToDelete.ForEach(a =>
            {
                _eventAggregator.PublishEvent(new AuthorDeletedEvent(a, deleteFiles, addImportListExclusion));
            });
        }

        public List<Author> GetAllAuthors()
        {
            return _authorRepository.All().ToList();
        }

        public List<string> AllAuthorForeignIds()
        {
            return _authorRepository.AllAuthorForeignIds();
        }

        public Dictionary<int, string> GetAllAuthorPaths()
        {
            return _authorRepository.AllAuthorPaths();
        }

        public Dictionary<int, List<int>> GetAllAuthorTags()
        {
            return _authorRepository.AllAuthorTags();
        }

        public List<Author> AllForTag(int tagId)
        {
            return GetAllAuthors().Where(a => a.Tags.Contains(tagId)).ToList();
        }

        public Author UpdateAuthor(Author author, bool publishUpdatedEvent = true)
        {
            var storedAuthor = GetAuthor(author.Id);

            var updatedAuthor = _authorRepository.Update(author);

            if (publishUpdatedEvent)
            {
                _eventAggregator.PublishEvent(new AuthorEditedEvent(updatedAuthor, storedAuthor));
            }

            return updatedAuthor;
        }

        public List<Author> UpdateAuthors(List<Author> authors, bool useExistingRelativeFolder)
        {
            _logger.Debug("Updating {0} authors", authors.Count);

            foreach (var author in authors)
            {
                _logger.Trace("Updating: {0}", author.Metadata.Value?.Name);

                if (!useExistingRelativeFolder && author.Path.IsNotNullOrWhiteSpace())
                {
                    author.Path = _authorPathBuilder.BuildPath(author, false);
                }

                _logger.Trace("Changing path for {0} to {1}", author.Metadata.Value?.Name, author.Path);
            }

            _authorRepository.UpdateMany(authors);
            _logger.Debug("{0} authors updated", authors.Count);

            return authors;
        }

        public bool AuthorPathExists(string folder)
        {
            return _authorRepository.AuthorPathExists(folder);
        }

        public void RemoveAddOptions(Author author)
        {
            _authorRepository.SetFields(author, a => a.AddOptions);
        }

        private bool UpdateTags(Author author)
        {
            var tagsAdded = new HashSet<int>();
            var tagsRemoved = new HashSet<int>();

            // TODO: Implement auto-tagging for authors
            // var changes = _autoTaggingService.GetTagChanges(author);

            // foreach (var tag in changes.TagsToAdd)
            // {
            //     if (!author.Tags.Contains(tag))
            //     {
            //         author.Tags.Add(tag);
            //         tagsAdded.Add(tag);
            //     }
            // }

            // foreach (var tag in changes.TagsToRemove)
            // {
            //     author.Tags.Remove(tag);
            //     tagsRemoved.Add(tag);
            // }

            if (tagsAdded.Any() || tagsRemoved.Any())
            {
                _authorRepository.Update(author);
                _logger.Debug("Updated tags for '{0}'. Added: {1}, Removed: {2}", author.Metadata.Value?.Name, tagsAdded.Count, tagsRemoved.Count);

                return true;
            }

            return false;
        }
    }
}