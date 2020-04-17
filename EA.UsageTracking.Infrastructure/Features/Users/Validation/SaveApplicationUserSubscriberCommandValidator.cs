using EA.UsageTracking.Infrastructure.Features.Users.Commands;
using EA.UsageTracking.SharedKernel.Constants;
using FluentValidation;

namespace EA.UsageTracking.Infrastructure.Features.Users.Validation
{
    public class SaveApplicationUserSubscriberCommandValidator : AbstractValidator<SaveApplicationUserSubscriberCommand>
    {
        public SaveApplicationUserSubscriberCommandValidator()
        {
            RuleFor(command => command.TenantId).NotNull().NotEmpty().WithMessage(Constants.ErrorMessages.NoTenant);
            RuleFor(command => command.IdentityToken).NotNull().NotEmpty().WithMessage(Constants.ErrorMessages.NoIdentityToken);
        }
    }
}