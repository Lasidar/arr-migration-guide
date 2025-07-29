using System.Collections.Generic;
using System.Linq;
using NLog;
using Readarr.Core.Messaging.Events;

namespace Readarr.Core.Books
{
    public class SeriesService : ISeriesService
    {
        private readonly ISeriesRepository _seriesRepository;
        private readonly IEventAggregator _eventAggregator;
        private readonly Logger _logger;

        public SeriesService(ISeriesRepository seriesRepository,
                            IEventAggregator eventAggregator,
                            Logger logger)
        {
            _seriesRepository = seriesRepository;
            _eventAggregator = eventAggregator;
            _logger = logger;
        }

        public Series GetSeries(int seriesId)
        {
            return _seriesRepository.Get(seriesId);
        }

        public List<Series> GetAllSeries()
        {
            return _seriesRepository.All().ToList();
        }

        public List<Series> GetByAuthorId(int authorId)
        {
            return _seriesRepository.GetByAuthorId(authorId);
        }

        public Series AddSeries(Series series)
        {
            _seriesRepository.Insert(series);
            _logger.Debug("Added book series {0}", series.Title);
            
            return series;
        }

        public Series UpdateSeries(Series series)
        {
            var updatedSeries = _seriesRepository.Update(series);
            _logger.Debug("Updated book series {0}", series.Title);
            
            return updatedSeries;
        }

        public void DeleteSeries(int seriesId)
        {
            var series = _seriesRepository.Get(seriesId);
            _seriesRepository.Delete(seriesId);
            _logger.Debug("Deleted book series {0}", series.Title);
        }

        public void LinkBookToSeries(int seriesId, int bookId, string position)
        {
            var link = new SeriesBookLink
            {
                SeriesId = seriesId,
                BookId = bookId,
                Position = position
            };

            _seriesRepository.InsertSeriesBookLink(link);
            _logger.Debug("Linked book {0} to series {1} at position {2}", bookId, seriesId, position);
        }

        public void UnlinkBookFromSeries(int seriesId, int bookId)
        {
            _seriesRepository.DeleteSeriesBookLink(seriesId, bookId);
            _logger.Debug("Unlinked book {0} from series {1}", bookId, seriesId);
        }

        public List<SeriesBookLink> GetSeriesBookLinks(int seriesId)
        {
            return _seriesRepository.GetSeriesBookLinks(seriesId);
        }

        public Series FindByForeignSeriesId(string foreignSeriesId)
        {
            return _seriesRepository.FindByForeignSeriesId(foreignSeriesId);
        }

        public Series FindByTitle(string title)
        {
            return _seriesRepository.FindByTitle(title);
        }

        public List<Series> FindByTitleInexact(string title)
        {
            return _seriesRepository.FindByTitleInexact(title);
        }

        public Dictionary<int, List<int>> GetAllSeriesBookIds()
        {
            return _seriesRepository.GetAllSeriesBookIds();
        }
    }
}