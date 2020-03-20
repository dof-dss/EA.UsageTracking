using System;
using System.Collections.Generic;
using System.Text;
using EA.UsageTracking.Infrastructure.Features.Usages.Commands;
using EA.UsageTracking.SharedKernel.Constants;
using FluentValidation;

namespace EA.UsageTracking.Infrastructure.Features.Usages.Validation
{
    public class AddUsageItemCommandValidator: AbstractValidator<AddUsageItemSubscriberCommand>
    {
        public AddUsageItemCommandValidator()
        {
            RuleFor(a => a.TenantId).NotNull().NotEqual(Guid.Empty)
                .WithMessage(Constants.ErrorMessages.EmptyGuid);
            RuleFor(a => a.ApplicationUserId).NotNull().NotEqual(Guid.Empty)
                .WithMessage(Constants.ErrorMessages.EmptyGuid);
            RuleFor(a => a.ApplicationEventId).NotEqual(0)
                .WithMessage(Constants.ErrorMessages.NoEventExists);
        }
    }
}
