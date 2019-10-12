using FluentValidation;
using FluentValidation.Results;
using System;
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

            RuleFor(site => site.Url)
                .Custom((url, context) =>
                {
                    Uri uri = null;
                    if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
                    {
                        context.AddFailure(new ValidationFailure(
                            nameof(Site.Url), "The site URL is invalid"));
                    }
                });
        }
    }
}
