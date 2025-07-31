using FluentValidation;
using Readarr.Core.Books;
using Readarr.Core.Datastore;
using System.Collections.Generic;
using System.Linq;
using Readarr.Core.Tv;

namespace Readarr.Core.Validation
{
    public class BookSeriesValidator : AbstractValidator<SeriesBookLink>
    {
        public BookSeriesValidator()
        {
            RuleFor(s => s.Position)
                .NotEmpty()
                .WithMessage("Series position is required")
                .Must(BeValidPosition)
                .WithMessage("Series position must be a valid number (e.g., '1', '2.5', '3')");
        }

        private bool BeValidPosition(string position)
        {
            if (string.IsNullOrWhiteSpace(position))
                return false;

            // Allow decimal positions for books that fall between others (e.g., 1.5)
            return decimal.TryParse(position, out var result) && result > 0;
        }
    }

    public class BookSeriesCollectionValidator : AbstractValidator<Book>
    {
        private readonly Books.ISeriesService _seriesService;

        public BookSeriesCollectionValidator(Tv.ISeriesService seriesService)
        {
            _seriesService = seriesService;

            RuleFor(b => b.SeriesLinks)
                .Must(NotHaveDuplicateSeries)
                .WithMessage("A book cannot be in the same series multiple times")
                .Must(NotHaveDuplicatePositions)
                .WithMessage("A book cannot have the same position in multiple series by the same author");
        }

        private bool NotHaveDuplicateSeries(LazyLoaded<List<SeriesBookLink>> seriesLinks)
        {
            if (!seriesLinks.IsLoaded || seriesLinks.Value == null || !seriesLinks.Value.Any())
                return true;

            var seriesIds = seriesLinks.Value.Select(s => s.SeriesId).ToList();
            return seriesIds.Count == seriesIds.Distinct().Count();
        }

        private bool NotHaveDuplicatePositions(LazyLoaded<List<SeriesBookLink>> seriesLinks)
        {
            if (!seriesLinks.IsLoaded || seriesLinks.Value == null || !seriesLinks.Value.Any())
                return true;

            // Group by series to check for position conflicts within each series
            var seriesGroups = seriesLinks.Value.GroupBy(s => s.SeriesId);
            
            foreach (var group in seriesGroups)
            {
                var series = _seriesService.GetSeries(group.Key);
                if (series == null) continue;

                // Check if any other books in this series have the same position
                var positions = group.Select(g => g.Position).ToList();
                if (positions.Count != positions.Distinct().Count())
                    return false;
            }

            return true;
        }
    }
}