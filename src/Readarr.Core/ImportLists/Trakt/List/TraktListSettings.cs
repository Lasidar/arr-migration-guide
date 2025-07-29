using FluentValidation;
using Readarr.Core.Annotations;
using Readarr.Core.Validation;

namespace Readarr.Core.ImportLists.Trakt.List
{
    public class TraktListSettingsValidator : TraktSettingsBaseValidator<TraktListSettings>
    {
        public TraktListSettingsValidator()
        {
            RuleFor(c => c.Username).NotEmpty();
            RuleFor(c => c.Listname).NotEmpty();
        }
    }

    public class TraktListSettings : TraktSettingsBase<TraktListSettings>
    {
        private static readonly TraktListSettingsValidator Validator = new();

        [FieldDefinition(1, Label = "Username", HelpText = "ImportListsTraktSettingsUsernameHelpText")]
        public string Username { get; set; }

        [FieldDefinition(2, Label = "ImportListsTraktSettingsListName", HelpText = "ImportListsTraktSettingsListNameHelpText")]
        public string Listname { get; set; }

        public override NzbDroneValidationResult Validate()
        {
            return new NzbDroneValidationResult(Validator.Validate(this));
        }
    }
}
