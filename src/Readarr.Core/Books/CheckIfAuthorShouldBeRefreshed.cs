using System;
using System.Linq;
using NLog;
using Readarr.Core.Configuration;

namespace Readarr.Core.Books
{
    public class CheckIfAuthorShouldBeRefreshed : ICheckIfAuthorShouldBeRefreshed
    {
        private readonly IAuthorService _authorService;
        private readonly IConfigService _configService;
        private readonly Logger _logger;

        public CheckIfAuthorShouldBeRefreshed(IAuthorService authorService,
                                             IConfigService configService,
                                             Logger logger)
        {
            _authorService = authorService;
            _configService = configService;
            _logger = logger;
        }

        public bool ShouldRefresh(int? authorId)
        {
            if (authorId.HasValue)
            {
                var author = _authorService.GetAuthor(authorId.Value);
                return ShouldRefresh(author);
            }

            return true;
        }

        private bool ShouldRefresh(Author author)
        {
            // Check if author needs refresh based on last refresh time
            var refreshInterval = _configService.MetadataRefreshInterval;
            
            if (author.LastInfoSync == null)
            {
                _logger.Trace($"Author {author.Name} has never been refreshed");
                return true;
            }

            var timeSinceLastRefresh = DateTime.UtcNow - author.LastInfoSync.Value;
            
            if (timeSinceLastRefresh > TimeSpan.FromDays(refreshInterval))
            {
                _logger.Trace($"Author {author.Name} last refreshed {timeSinceLastRefresh.TotalDays:N1} days ago, should refresh");
                return true;
            }

            _logger.Trace($"Author {author.Name} last refreshed {timeSinceLastRefresh.TotalDays:N1} days ago, skipping");
            return false;
        }
    }
}