using FluentValidation.Validators;
using Readarr.Common.Disk;

namespace Readarr.Core.Validation
{
    public class FolderChmodValidator : PropertyValidator
    {
        private readonly IDiskProvider _diskProvider;

        public FolderChmodValidator(IDiskProvider diskProvider)
        {
            _diskProvider = diskProvider;
        }

        protected override string GetDefaultMessageTemplate() => "Must contain a valid Unix permissions octal";

        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context.PropertyValue == null)
            {
                return false;
            }

            return _diskProvider.IsValidFolderPermissionMask(context.PropertyValue.ToString());
        }
    }
}
