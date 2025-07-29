﻿using Readarr.Common.Extensions;
using Readarr.Core.DataAugmentation.Scene;

namespace Readarr.Api.V1.Series
{
    public class AlternateTitleResource
    {
        public string Title { get; set; }
        public int? SeasonNumber { get; set; }
        public int? SceneSeasonNumber { get; set; }
        public string SceneOrigin { get; set; }
        public string Comment { get; set; }
    }

    public static class AlternateTitleResourceMapper
    {
        public static AlternateTitleResource ToResource(this SceneMapping sceneMapping)
        {
            if (sceneMapping == null)
            {
                return null;
            }

            var comment = sceneMapping.Comment;

            if (comment.IsNullOrWhiteSpace() && sceneMapping.FilterRegex.IsNotNullOrWhiteSpace())
            {
                comment = "Limited matching";
            }

            return new AlternateTitleResource
            {
                Title = sceneMapping.Title,
                SeasonNumber = sceneMapping.SeasonNumber,
                SceneSeasonNumber = sceneMapping.SceneSeasonNumber,
                SceneOrigin = sceneMapping.SceneOrigin,
                Comment = comment
            };
        }
    }
}
