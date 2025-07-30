using System.Collections.Generic;
using System.Linq;
using NLog;
using Readarr.Core.Messaging.Events;

namespace Readarr.Core.Books
{
    public class EditionService : IEditionService
    {
        private readonly IEditionRepository _editionRepository;
        private readonly IEventAggregator _eventAggregator;
        private readonly Logger _logger;

        public EditionService(IEditionRepository editionRepository,
                             IEventAggregator eventAggregator,
                             Logger logger)
        {
            _editionRepository = editionRepository;
            _eventAggregator = eventAggregator;
            _logger = logger;
        }

        public Edition GetEdition(int editionId)
        {
            return _editionRepository.Get(editionId);
        }

        public List<Edition> GetEditions(int bookId)
        {
            return _editionRepository.GetEditions(bookId);
        }

        public List<Edition> GetAllEditions()
        {
            return _editionRepository.All().ToList();
        }

        public Edition GetEditionByFileId(int fileId)
        {
            return _editionRepository.GetEditionByFileId(fileId);
        }

        public Edition UpdateEdition(Edition edition)
        {
            var storedEdition = GetEdition(edition.Id);
            var updatedEdition = _editionRepository.Update(edition);
            
            _logger.Debug("Updated edition {0}", edition.Title);

            return updatedEdition;
        }

        public void SetMonitored(IEnumerable<int> ids, bool monitored)
        {
            _editionRepository.SetMonitored(ids, monitored);
        }

        public Edition FindByForeignEditionId(string foreignEditionId)
        {
            return _editionRepository.FindByForeignEditionId(foreignEditionId);
        }

        public Edition FindByIsbn(string isbn)
        {
            return _editionRepository.FindByIsbn(isbn);
        }

        public List<Edition> GetEditionsByAuthor(int authorId)
        {
            return _editionRepository.GetEditionsByAuthor(authorId);
        }

        public List<Edition> GetEditionsByBookIds(List<int> bookIds)
        {
            return _editionRepository.GetEditionsByBookIds(bookIds);
        }

        public List<Edition> EditionsWithFiles(int authorId)
        {
            return _editionRepository.EditionsWithFiles(authorId);
        }

        public List<Edition> EditionsWithoutFiles(int authorId)
        {
            return _editionRepository.EditionsWithoutFiles(authorId);
        }

        public void InsertMany(List<Edition> editions)
        {
            _editionRepository.InsertMany(editions);
        }

        public void UpdateMany(List<Edition> editions)
        {
            _editionRepository.UpdateMany(editions);
        }

        public void DeleteMany(List<Edition> editions)
        {
            _editionRepository.DeleteMany(editions.Select(e => e.Id).ToList());
        }

        public void SetFileId(int editionId, int fileId)
        {
            var edition = _editionRepository.Get(editionId);
            _editionRepository.SetFileId(edition, fileId);
        }

        public void ClearFileId(int editionId)
        {
            var edition = _editionRepository.Get(editionId);
            _editionRepository.ClearFileId(edition, false);
        }
    }
}