using EA.UsageTracking.Infrastructure.Features.Events.Commands;
using EA.UsageTracking.SharedKernel.Constants;
using FluentValidation;

namespace EA.UsageTracking.Infrastructure.Features.Events.Validation
{
    public class AddApplicationEventCommandValidator : AbstractValidator<AddApplicationEventCommand>
    {
        public AddApplicationEventCommandValidator()
        {
            RuleFor(command => command.ApplicationEventDto.Name).NotNull().NotEmpty().WithMessage(Constants.ErrorMessages.NoEventName);
        }
    }
}