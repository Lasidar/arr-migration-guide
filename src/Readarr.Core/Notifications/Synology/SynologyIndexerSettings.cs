using FluentValidation;
using Readarr.Core.Annotations;
using Readarr.Core.Validation;

namespace Readarr.Core.Notifications.Synology
{
    public class SynologyIndexerSettingsValidator : AbstractValidator<SynologyIndexerSettings>
    {
    }

    public class SynologyIndexerSettings : NotificationSettingsBase<SynologyIndexerSettings>
    {
        private static readonly SynologyIndexerSettingsValidator Validator = new();

        public SynologyIndexerSettings()
        {
            UpdateLibrary = true;
        }

        [FieldDefinition(0, Label = "NotificationsSettingsUpdateLibrary", Type = FieldType.Checkbox, HelpText = "NotificationsSynologySettingsUpdateLibraryHelpText")]
        public bool UpdateLibrary { get; set; }

        public override NzbDroneValidationResult Validate()
        {
            return new NzbDroneValidationResult(Validator.Validate(this));
        }
    }
}
