using System;
using System.Collections.Generic;
using System.Text;
using EA.UsageTracking.Infrastructure.Features.Usages.Queries;
using EA.UsageTracking.SharedKernel.Constants;
using FluentValidation;

namespace EA.UsageTracking.Infrastructure.Features.Usages.Validation
{
    public class GetUsagesForUserValidator: AbstractValidator<GetUsagesForUserQuery>
    {
        public GetUsagesForUserValidator()
        {
            RuleFor(q => q.PageNumber).GreaterThanOrEqualTo(1).WithMessage(Constants.ErrorMessages.InvalidPageNumber);
            RuleFor(q => q.PageSize).GreaterThanOrEqualTo(1).WithMessage(Constants.ErrorMessages.InvalidPageSize);
        }
    }
}
