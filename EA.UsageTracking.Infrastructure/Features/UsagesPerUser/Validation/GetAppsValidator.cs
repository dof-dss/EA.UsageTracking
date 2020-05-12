using EA.UsageTracking.Infrastructure.Features.UsagesPerApplication.Queries;
using EA.UsageTracking.Infrastructure.Features.UsagesPerUser.Queries;
using EA.UsageTracking.SharedKernel.Constants;
using FluentValidation;

namespace EA.UsageTracking.Infrastructure.Features.UsagesPerUser.Validation
{
    public class GetAppsValidator: AbstractValidator<GetAppsQuery>
    {
        public GetAppsValidator()
        {
            RuleFor(q => q.PageNumber).GreaterThanOrEqualTo(1).WithMessage(Constants.ErrorMessages.InvalidPageNumber);
            RuleFor(q => q.PageSize).GreaterThanOrEqualTo(1).WithMessage(Constants.ErrorMessages.InvalidPageSize);
        }
    }
}
