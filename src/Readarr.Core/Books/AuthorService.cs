using System.Collections.Generic;
using System.Linq;
using NLog;
using Readarr.Common.Extensions;
using Readarr.Core.AutoTagging;
using Readarr.Core.Books.Events;
using Readarr.Core.Messaging.Events;
using Readarr.Core.Parser;
using System;

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
        List<Author> FindDuplicateAuthors(string authorName);
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
            _logger.Debug("Adding {0} authors", newAuthors.Count);
            
            newAuthors.ForEach(author =>
            {
                author.Added = DateTime.UtcNow;
                author.CleanName = author.Metadata.Value.Name.CleanAuthorName();
                author.SortName = Parser.Parser.NormalizeTitle(author.Metadata.Value.Name);
                author.Added = DateTime.UtcNow;
            });

            // Use a transaction to ensure atomicity
            var addedAuthors = new List<Author>();
            
            try
            {
                // Note: Transaction support would need to be added to the repository layer
                // For now, we'll add error handling and rollback logic
                foreach (var author in newAuthors)
                {
                    try
                    {
                        var addedAuthor = _authorRepository.Insert(author);
                        addedAuthors.Add(addedAuthor);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "Failed to add author {0}", author.Metadata.Value?.Name);
                        // Remove any authors that were successfully added
                        if (addedAuthors.Any())
                        {
                            _logger.Debug("Rolling back {0} authors", addedAuthors.Count);
                            _authorRepository.DeleteMany(addedAuthors);
                        }
                        throw;
                    }
                }

                _eventAggregator.PublishEvent(new AuthorsImportedEvent(addedAuthors));
                
                return addedAuthors;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to add authors");
                throw;
            }
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

        public bool UpdateTags(Author author)
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

        public List<Author> FindDuplicateAuthors(string authorName)
        {
            if (string.IsNullOrWhiteSpace(authorName))
                return new List<Author>();

            var cleanName = authorName.CleanAuthorName();
            var normalizedName = Parser.Parser.NormalizeTitle(authorName);
            
            var allAuthors = GetAllAuthors();
            var duplicates = new List<Author>();

            foreach (var author in allAuthors)
            {
                var existingCleanName = author.Metadata.Value?.Name?.CleanAuthorName();
                var existingNormalizedName = author.SortName;

                // Exact match on clean name
                if (cleanName.Equals(existingCleanName, StringComparison.OrdinalIgnoreCase))
                {
                    duplicates.Add(author);
                    continue;
                }

                // Exact match on normalized name
                if (normalizedName.Equals(existingNormalizedName, StringComparison.OrdinalIgnoreCase))
                {
                    duplicates.Add(author);
                    continue;
                }

                // Fuzzy matching - check for common variations
                if (AreSimilarAuthorNames(authorName, author.Metadata.Value?.Name))
                {
                    duplicates.Add(author);
                }
            }

            return duplicates;
        }

        private bool AreSimilarAuthorNames(string name1, string name2)
        {
            if (string.IsNullOrWhiteSpace(name1) || string.IsNullOrWhiteSpace(name2))
                return false;

            // Remove common punctuation and normalize
            var clean1 = name1.Replace(".", "").Replace(",", "").Replace("'", "").Trim().ToLowerInvariant();
            var clean2 = name2.Replace(".", "").Replace(",", "").Replace("'", "").Trim().ToLowerInvariant();

            // Check if one is contained in the other (handles "J.K. Rowling" vs "JK Rowling")
            if (clean1.Contains(clean2) || clean2.Contains(clean1))
                return true;

            // Check for reversed names (handles "Lastname, Firstname" vs "Firstname Lastname")
            var parts1 = clean1.Split(' ');
            var parts2 = clean2.Split(' ');

            if (parts1.Length == 2 && parts2.Length == 2)
            {
                if ((parts1[0] == parts2[1] && parts1[1] == parts2[0]) ||
                    (parts1[0] == parts2[0] && parts1[1] == parts2[1]))
                {
                    return true;
                }
            }

            // Check Levenshtein distance for typos
            var distance = ComputeLevenshteinDistance(clean1, clean2);
            var maxLength = Math.Max(clean1.Length, clean2.Length);
            
            // If the edit distance is less than 10% of the length, consider it similar
            return distance <= maxLength * 0.1;
        }

        private int ComputeLevenshteinDistance(string s1, string s2)
        {
            var distances = new int[s1.Length + 1, s2.Length + 1];

            for (int i = 0; i <= s1.Length; i++)
                distances[i, 0] = i;
            
            for (int j = 0; j <= s2.Length; j++)
                distances[0, j] = j;

            for (int i = 1; i <= s1.Length; i++)
            {
                for (int j = 1; j <= s2.Length; j++)
                {
                    var cost = s1[i - 1] == s2[j - 1] ? 0 : 1;
                    distances[i, j] = Math.Min(
                        Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                        distances[i - 1, j - 1] + cost
                    );
                }
            }

            return distances[s1.Length, s2.Length];
        }
    }
}