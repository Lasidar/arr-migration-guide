using FluentValidation;
using Readarr.Core.Annotations;
using Readarr.Core.Validation;
using Readarr.Core.Validation.Paths;

namespace Readarr.Core.Download.Clients.Blackhole
{
    public class UsenetBlackholeSettingsValidator : AbstractValidator<UsenetBlackholeSettings>
    {
        public UsenetBlackholeSettingsValidator()
        {
            RuleFor(c => c.NzbFolder).IsValidPath();
            RuleFor(c => c.WatchFolder).IsValidPath();
        }
    }

    public class UsenetBlackholeSettings : DownloadClientSettingsBase<UsenetBlackholeSettings>
    {
        private static readonly UsenetBlackholeSettingsValidator Validator = new();

        [FieldDefinition(0, Label = "UsenetBlackholeNzbFolder", Type = FieldType.Path, HelpText = "BlackholeFolderHelpText")]
        [FieldToken(TokenField.HelpText, "UsenetBlackholeNzbFolder", "extension", ".nzb")]
        public string NzbFolder { get; set; }

        [FieldDefinition(1, Label = "BlackholeWatchFolder", Type = FieldType.Path, HelpText = "BlackholeWatchFolderHelpText")]
        public string WatchFolder { get; set; }

        public override NzbDroneValidationResult Validate()
        {
            return new NzbDroneValidationResult(Validator.Validate(this));
        }
    }
}
