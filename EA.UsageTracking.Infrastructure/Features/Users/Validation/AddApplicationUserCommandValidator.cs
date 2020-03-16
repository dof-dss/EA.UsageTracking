using EA.UsageTracking.Infrastructure.Features.Users.Commands;
using EA.UsageTracking.SharedKernel.Constants;
using FluentValidation;

namespace EA.UsageTracking.Infrastructure.Features.Users.Validation
{
    public class AddApplicationUserCommandValidator : AbstractValidator<AddApplicationUserCommand>
    {
        public AddApplicationUserCommandValidator()
        {
            RuleFor(command => command.ApplicationUserDto.Id).NotNull().NotEmpty().WithMessage(Constants.ErrorMessages.EmptyGuid);
            RuleFor(command => command.ApplicationUserDto.Name).NotNull().NotEmpty().WithMessage(Constants.ErrorMessages.NoUserName);
        }
    }
}