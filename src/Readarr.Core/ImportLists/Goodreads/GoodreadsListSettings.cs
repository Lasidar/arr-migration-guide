using FluentValidation;
using Readarr.Core.Annotations;
using Readarr.Core.Validation;

namespace Readarr.Core.ImportLists.Goodreads
{
    public class GoodreadsListSettingsValidator : AbstractValidator<GoodreadsListSettings>
    {
        public GoodreadsListSettingsValidator()
        {
            RuleFor(c => c.ListId).NotEmpty();
            RuleFor(c => c.ApiKey).NotEmpty();
        }
    }

    public class GoodreadsListSettings : ImportListSettingsBase<GoodreadsListSettings>
    {
        private static readonly GoodreadsListSettingsValidator Validator = new GoodreadsListSettingsValidator();

        [FieldDefinition(0, Label = "List ID", HelpText = "Goodreads list ID (e.g., 123456)")]
        public string ListId { get; set; }

        [FieldDefinition(1, Label = "API Key", Privacy = PrivacyLevel.ApiKey, HelpText = "Goodreads API Key")]
        public string ApiKey { get; set; }

        public override NzbDroneValidationResult Validate()
        {
            return new NzbDroneValidationResult(Validator.Validate(this));
        }
    }
}