﻿using System;
using System.Collections.Generic;
using System.Text;
using EA.UsageTracking.Infrastructure.Features.Events.Queries;
using EA.UsageTracking.SharedKernel.Constants;
using FluentValidation;

namespace EA.UsageTracking.Infrastructure.Features.Events.Validation
{
    public class GetEventsForApplicationValidator: AbstractValidator<GetEventsForApplicationQuery>
    {
        public GetEventsForApplicationValidator()
        {
            RuleFor(q => q.PageNumber).GreaterThanOrEqualTo(1).WithMessage(Constants.ErrorMessages.InvalidPageNumber);
            RuleFor(q => q.PageSize).GreaterThanOrEqualTo(1).WithMessage(Constants.ErrorMessages.InvalidPageSize);
        }
    }
}
