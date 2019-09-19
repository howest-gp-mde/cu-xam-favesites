using FluentValidation;
using XrnCourse.FavoriteSites.Domain.Models;

namespace XrnCourse.FavoriteSites.Domain.Validators
{
    public class SiteValidator : AbstractValidator<Site>
    {
        public SiteValidator()
        {
            RuleFor(site => site.Name)
                .NotEmpty()
                .WithMessage($"{nameof(Site.Name)} cannot be empty")
                .Length(3, 30)
                .WithMessage($"{nameof(Site.Name)} must be between 3 and 30");

        }
    }
}
