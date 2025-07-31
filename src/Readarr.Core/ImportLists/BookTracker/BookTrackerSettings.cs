using FluentValidation;
using Readarr.Core.Annotations;
using Readarr.Core.Validation;

namespace Readarr.Core.ImportLists.BookTracker
{
    public class BookTrackerSettingsValidator : AbstractValidator<BookTrackerSettings>
    {
        public BookTrackerSettingsValidator()
        {
            RuleFor(c => c.BaseUrl).NotEmpty();
            RuleFor(c => c.Username).NotEmpty();
            RuleFor(c => c.ListId).NotEmpty();
        }
    }

    public class BookTrackerSettings : ImportListSettingsBase<BookTrackerSettings>
    {
        private static readonly BookTrackerSettingsValidator Validator = new BookTrackerSettingsValidator();

        [FieldDefinition(0, Label = "URL", HelpText = "Book tracker API URL")]
        public string BaseUrl { get; set; }

        [FieldDefinition(1, Label = "Username", HelpText = "Username for the book tracking service")]
        public string Username { get; set; }

        [FieldDefinition(2, Label = "List ID", HelpText = "The list to import (e.g., 'reading', 'to-read', 'wishlist')")]
        public string ListId { get; set; }

        [FieldDefinition(3, Label = "API Key", Privacy = PrivacyLevel.ApiKey, HelpText = "API key for authentication (if required)")]
        public string ApiKey { get; set; }

        public override NzbDroneValidationResult Validate()
        {
            return new NzbDroneValidationResult(Validator.Validate(this));
        }
    }
}