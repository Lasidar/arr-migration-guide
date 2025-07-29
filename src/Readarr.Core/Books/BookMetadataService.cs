using System.Collections.Generic;
using System.Linq;
using NLog;
using Readarr.Core.Messaging.Events;

namespace Readarr.Core.Books
{
    public class BookMetadataService : IBookMetadataService
    {
        private readonly IBookMetadataRepository _bookMetadataRepository;
        private readonly IEventAggregator _eventAggregator;
        private readonly Logger _logger;

        public BookMetadataService(IBookMetadataRepository bookMetadataRepository,
                                  IEventAggregator eventAggregator,
                                  Logger logger)
        {
            _bookMetadataRepository = bookMetadataRepository;
            _eventAggregator = eventAggregator;
            _logger = logger;
        }

        public BookMetadata Get(int id)
        {
            return _bookMetadataRepository.Get(id);
        }

        public BookMetadata FindById(string foreignId)
        {
            return _bookMetadataRepository.FindByForeignBookId(foreignId);
        }

        public BookMetadata Upsert(BookMetadata metadata)
        {
            var existing = FindById(metadata.ForeignBookId);
            
            if (existing != null)
            {
                metadata.Id = existing.Id;
                _logger.Debug("Updating book metadata for {0}", metadata.Title);
                return _bookMetadataRepository.Update(metadata);
            }
            else
            {
                _logger.Debug("Inserting book metadata for {0}", metadata.Title);
                _bookMetadataRepository.Insert(metadata);
                return metadata;
            }
        }

        public List<BookMetadata> UpsertMany(List<BookMetadata> metadataList)
        {
            var toInsert = new List<BookMetadata>();
            var toUpdate = new List<BookMetadata>();

            foreach (var metadata in metadataList)
            {
                var existing = FindById(metadata.ForeignBookId);
                
                if (existing != null)
                {
                    metadata.Id = existing.Id;
                    toUpdate.Add(metadata);
                }
                else
                {
                    toInsert.Add(metadata);
                }
            }

            if (toInsert.Any())
            {
                _bookMetadataRepository.InsertMany(toInsert);
            }

            if (toUpdate.Any())
            {
                _bookMetadataRepository.UpdateMany(toUpdate);
            }

            return metadataList;
        }
    }
}