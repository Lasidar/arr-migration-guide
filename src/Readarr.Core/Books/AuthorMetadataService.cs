using System.Collections.Generic;
using System.Linq;
using NLog;
using Readarr.Core.Messaging.Events;

namespace Readarr.Core.Books
{
    public class AuthorMetadataService : IAuthorMetadataService
    {
        private readonly IAuthorMetadataRepository _authorMetadataRepository;
        private readonly IEventAggregator _eventAggregator;
        private readonly Logger _logger;

        public AuthorMetadataService(IAuthorMetadataRepository authorMetadataRepository,
                                    IEventAggregator eventAggregator,
                                    Logger logger)
        {
            _authorMetadataRepository = authorMetadataRepository;
            _eventAggregator = eventAggregator;
            _logger = logger;
        }

        public AuthorMetadata Get(int id)
        {
            return _authorMetadataRepository.Get(id);
        }

        public AuthorMetadata FindById(string foreignId)
        {
            return _authorMetadataRepository.FindByForeignAuthorId(foreignId);
        }

        public AuthorMetadata Upsert(AuthorMetadata metadata)
        {
            var existing = FindById(metadata.ForeignAuthorId);
            
            if (existing != null)
            {
                metadata.Id = existing.Id;
                _logger.Debug("Updating author metadata for {0}", metadata.Name);
                return _authorMetadataRepository.Update(metadata);
            }
            else
            {
                _logger.Debug("Inserting author metadata for {0}", metadata.Name);
                _authorMetadataRepository.Insert(metadata);
                return metadata;
            }
        }

        public List<AuthorMetadata> UpsertMany(List<AuthorMetadata> metadataList)
        {
            var toInsert = new List<AuthorMetadata>();
            var toUpdate = new List<AuthorMetadata>();

            foreach (var metadata in metadataList)
            {
                var existing = FindById(metadata.ForeignAuthorId);
                
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
                _authorMetadataRepository.InsertMany(toInsert);
            }

            if (toUpdate.Any())
            {
                _authorMetadataRepository.UpdateMany(toUpdate);
            }

            return metadataList;
        }
    }
}