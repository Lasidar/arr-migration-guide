using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using NLog;
using Readarr.Common.Disk;
using Readarr.Common.EnsureThat;
using Readarr.Core.Books.Events;
using Readarr.Core.Exceptions;
using Readarr.Core.MetadataSource;
using Readarr.Core.Messaging.Events;
using Readarr.Core.Organizer;
using Readarr.Core.Parser;
using Readarr.Core.Validation;
using Readarr.Core.Validation.Paths;
using Readarr.Core.Tv;

namespace Readarr.Core.Books
{
    public class AddAuthorService : IAddAuthorService
    {
        private readonly IAuthorService _authorService;
        private readonly IAuthorMetadataService _authorMetadataService;
        private readonly IProvideAuthorInfo _authorInfo;
        private readonly IBuildFileNames _fileNameBuilder;
        private readonly IAddAuthorValidator _addAuthorValidator;
        private readonly IEventAggregator _eventAggregator;
        private readonly Logger _logger;

        public AddAuthorService(IAuthorService authorService,
                               IAuthorMetadataService authorMetadataService,
                               IProvideAuthorInfo authorInfo,
                               IBuildFileNames fileNameBuilder,
                               IAddAuthorValidator addAuthorValidator,
                               IEventAggregator eventAggregator,
                               Logger logger)
        {
            _authorService = authorService;
            _authorMetadataService = authorMetadataService;
            _authorInfo = authorInfo;
            _fileNameBuilder = fileNameBuilder;
            _addAuthorValidator = addAuthorValidator;
            _eventAggregator = eventAggregator;
            _logger = logger;
        }

        public Author AddAuthor(Author newAuthor)
        {
            Ensure.That(newAuthor, () => newAuthor).IsNotNull();

            newAuthor = AddSkyhookData(newAuthor);
            newAuthor = SetPropertiesAndValidate(newAuthor);

            _logger.Info("Adding Author {0} Path: [{1}]", newAuthor, newAuthor.Path);

            // Add metadata
            _authorMetadataService.Upsert(newAuthor.Metadata.Value);
            newAuthor.AuthorMetadataId = newAuthor.Metadata.Value.Id;

            // Add the author
            _authorService.AddAuthor(newAuthor);

            return newAuthor;
        }

        private Author AddSkyhookData(Author newAuthor)
        {
            var author = newAuthor;

            try
            {
                var tuple = _authorInfo.GetAuthorInfo(newAuthor.Metadata.Value.ForeignAuthorId);
                var authorInfo = tuple.Item1;

                author.Metadata = authorInfo.Metadata;
                author.Books = authorInfo.Books;
                author.Series = authorInfo.Series;
            }
            catch (AuthorNotFoundException)
            {
                _logger.Error("Foreign ID {0} was not found, it may have been removed from Goodreads.", newAuthor.Metadata.Value.ForeignAuthorId);

                throw new ValidationException(new List<ValidationFailure>
                {
                    new ValidationFailure("ForeignAuthorId", $"An author with this ID was not found. It may have been removed from Goodreads.", newAuthor.Metadata.Value.ForeignAuthorId)
                });
            }

            return author;
        }

        private Author SetPropertiesAndValidate(Author newAuthor)
        {
            if (string.IsNullOrWhiteSpace(newAuthor.Path))
            {
                var folderName = _fileNameBuilder.GetAuthorFolder(newAuthor);
                newAuthor.Path = Path.Combine(newAuthor.RootFolderPath, folderName);
            }

            newAuthor.CleanName = newAuthor.Metadata.Value.Name.CleanAuthorName();
            newAuthor.SortName = newAuthor.Metadata.Value.SortName;
            newAuthor.Added = DateTime.UtcNow;

            if (newAuthor.AddOptions != null && newAuthor.AddOptions.Monitor == MonitorTypes.None)
            {
                newAuthor.Monitored = false;
            }

            var validationResult = _addAuthorValidator.Validate(newAuthor);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            return newAuthor;
        }
    }

    public interface IAddAuthorValidator
    {
        ValidationResult Validate(Author instance);
    }

    public class AddAuthorValidator : AbstractValidator<Author>, IAddAuthorValidator
    {
        public AddAuthorValidator(IAuthorService authorService,
                                 AuthorAncestorValidator authorAncestorValidator,
                                 AuthorPathValidator pathExistsValidator)
        {
            RuleFor(a => a.Metadata.Value).NotNull().SetValidator(new AuthorMetadataValidator());
            RuleFor(a => a.Path).Cascade(CascadeMode.Stop)
                                .NotEmpty()
                                .SetValidator(authorAncestorValidator)
                                .SetValidator(pathExistsValidator);
                                // TODO: Fix QualityProfileExistsValidator
                    // RuleFor(a => a.QualityProfileId).SetValidator(new QualityProfileExistsValidator());
            // TODO: Fix MetadataProfileExistsValidator
            // RuleFor(a => a.MetadataProfileId).SetValidator(new MetadataProfileExistsValidator());
        }
    }

    public class AuthorMetadataValidator : AbstractValidator<AuthorMetadata>
    {
        public AuthorMetadataValidator()
        {
            RuleFor(a => a.ForeignAuthorId).NotEmpty();
            RuleFor(a => a.Name).NotEmpty();
        }
    }

    public interface IAuthorAncestorValidator
    {
    }

    public interface IAuthorPathValidator
    {
    }

    // TODO: Fix PropertyValidator base class
    /*
    public class QualityProfileExistsValidator : PropertyValidator
    {
        protected override string GetDefaultMessageTemplate() => "Quality Profile does not exist";

        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context.PropertyValue == null)
            {
                return false;
            }

            // TODO: Implement quality profile check
            return true;
        }
    }
    */
}