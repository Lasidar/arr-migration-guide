using System.Collections.Generic;

namespace Readarr.Core.AuthorStats
{
    public interface IAuthorStatisticsService
    {
        List<AuthorStatistics> AuthorStatistics();
        AuthorStatistics GetAuthorStatistics(int authorId);
    }
}