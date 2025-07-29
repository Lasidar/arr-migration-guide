using FluentValidation;
using Readarr.Core.Configuration;
using Readarr.Core.ImportLists;
using Readarr.Core.Validation;
using Readarr.Http;

namespace Readarr.Api.V1.Config
{
    [V3ApiController("config/importlist")]

    public class ImportListConfigController : ConfigController<ImportListConfigResource>
    {
        public ImportListConfigController(IConfigService configService)
            : base(configService)
        {
            SharedValidator.RuleFor(x => x.ListSyncTag)
               .ValidId()
               .WithMessage("Tag must be specified")
               .When(x => x.ListSyncLevel == ListSyncLevelType.KeepAndTag);
        }

        protected override ImportListConfigResource ToResource(IConfigService model)
        {
            return ImportListConfigResourceMapper.ToResource(model);
        }
    }
}
