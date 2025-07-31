using FluentValidation;
using Readarr.Core.Annotations;
using Readarr.Core.Validation;

namespace Readarr.Core.ImportLists.Custom
{
    public class CustomBookSettingsValidator : AbstractValidator<CustomBookSettings>
    {
        public CustomBookSettingsValidator()
        {
            RuleFor(c => c.BaseUrl).NotEmpty();
        }
    }

    public class CustomBookSettings : ImportListSettingsBase<CustomBookSettings>
    {
        private static readonly CustomBookSettingsValidator Validator = new CustomBookSettingsValidator();

        [FieldDefinition(0, Label = "List URL", HelpText = "The URL for the custom book list")]
        public string BaseUrl { get; set; }

        public override NzbDroneValidationResult Validate()
        {
            return new NzbDroneValidationResult(Validator.Validate(this));
        }
    }
}