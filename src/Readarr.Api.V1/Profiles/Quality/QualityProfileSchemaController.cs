using Microsoft.AspNetCore.Mvc;
using Readarr.Core.Profiles.Qualities;
using Readarr.Http;

namespace Readarr.Api.V1.Profiles.Quality
{
    [V3ApiController("qualityprofile/schema")]
    public class QualityProfileSchemaController : Controller
    {
        private readonly IQualityProfileService _profileService;

        public QualityProfileSchemaController(IQualityProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        public QualityProfileResource GetSchema()
        {
            var qualityProfile = _profileService.GetDefaultProfile(string.Empty);

            return qualityProfile.ToResource();
        }
    }
}
