using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Readarr.Core.Languages;
using Readarr.Http;
using Readarr.Http.REST;

namespace Readarr.Api.V1.Profiles.Languages
{
    [V3ApiController("languageprofile/schema")]
    [Obsolete("Deprecated")]
    public class LanguageProfileSchemaController : RestController<LanguageProfileResource>
    {
        [HttpGet]
        [Produces("application/json")]
        public LanguageProfileResource GetSchema()
        {
            return new LanguageProfileResource
            {
                Id = 1,
                Name = "Deprecated",
                UpgradeAllowed = true,
                Cutoff = Language.English,
                Languages = new List<LanguageProfileItemResource>
                {
                    new LanguageProfileItemResource
                    {
                        Language = Language.English,
                        Allowed = true
                    }
                }
            };
        }

        [NonAction]
        public override ActionResult<LanguageProfileResource> GetResourceByIdWithErrorHandler(int id)
        {
            return base.GetResourceByIdWithErrorHandler(id);
        }

        protected override LanguageProfileResource GetResourceById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
