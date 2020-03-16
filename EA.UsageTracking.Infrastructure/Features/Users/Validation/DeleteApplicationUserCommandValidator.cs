using EA.UsageTracking.Infrastructure.Features.Users.Commands;
using EA.UsageTracking.SharedKernel.Constants;
using FluentValidation;

namespace EA.UsageTracking.Infrastructure.Features.Users.Validation
{
    public class DeleteApplicationUserCommandValidator : AbstractValidator<DeleteApplicationUserCommand>
    {
        public DeleteApplicationUserCommandValidator()
        {
            RuleFor(command => command.Id).NotNull().NotEmpty().WithMessage(Constants.ErrorMessages.EmptyGuid);
        }
    }
}