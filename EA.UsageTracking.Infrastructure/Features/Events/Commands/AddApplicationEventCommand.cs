using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.Infrastructure.Features.Common;
using EA.UsageTracking.Infrastructure.Features.Events.Validation;
using EA.UsageTracking.SharedKernel;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Extensions;
using EA.UsageTracking.SharedKernel.Functional;
using MediatR;

namespace EA.UsageTracking.Infrastructure.Features.Events.Commands
{
    public class AddApplicationEventCommand: IRequest<Result<ApplicationEventDTO>>
    {
        public ApplicationEventDTO ApplicationEventDto { get; set; }
    }

    public class AddApplicationEventCommandHandler : 
        AsyncBaseHandler<AddApplicationEventCommand>, 
        IRequestHandler<AddApplicationEventCommand, Result<ApplicationEventDTO>>
    {
        private readonly AddApplicationEventCommandValidator _validator;

        public AddApplicationEventCommandHandler(IUsageTrackingContextFactory usageTrackingContextFactory, IMapper mapper)
        : base(usageTrackingContextFactory, mapper)
        {
            _validator = new AddApplicationEventCommandValidator();
        }

        public async Task<Result<ApplicationEventDTO>> Handle(AddApplicationEventCommand request, CancellationToken cancellationToken)
        {
            var validationResults = Validate(request);
            if (validationResults.IsFailure) 
                return Result.Fail<ApplicationEventDTO>(validationResults.Error);

            var applicationEvent = Mapper.Map<ApplicationEvent>(request.ApplicationEventDto);
            applicationEvent.Application = DbContext.Applications.Single();

            DbContext.ApplicationEvents.Add(applicationEvent);
            await DbContext.SaveChangesAsync(cancellationToken);

            return Result.Ok(Mapper.Map<ApplicationEventDTO>(applicationEvent));
        }

        protected override Result CustomValidate(AddApplicationEventCommand request)
        {
            var validationResults = _validator.Validate(request);
            return (!validationResults.IsValid? Result.Fail(validationResults.ToString(",")) : Result.Ok());
        }
    }
}
