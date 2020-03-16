using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.Infrastructure.Features.Common;
using EA.UsageTracking.SharedKernel;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Extensions;
using EA.UsageTracking.SharedKernel.Functional;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EA.UsageTracking.Infrastructure.Features.Events.Commands
{
    public class UpdateApplicationEventCommand: IRequest<Result<ApplicationEventDTO>>
    {
        public ApplicationEventDTO ApplicationEventDto { get; set; }
    }

    public class UpdateApplicationEventCommandHandler : AsyncBaseHandler<UpdateApplicationEventCommand>, IRequestHandler<UpdateApplicationEventCommand,Result<ApplicationEventDTO>>
    {
        public UpdateApplicationEventCommandHandler(IUsageTrackingContextFactory usageTrackingContextFactory, IMapper mapper) :
            base(usageTrackingContextFactory, mapper)
        {}

        public async Task<Result<ApplicationEventDTO>> Handle(UpdateApplicationEventCommand request, CancellationToken cancellationToken)
        {
            var validationResults = Validate(request);
            if (validationResults.IsFailure)
                return Result.Fail<ApplicationEventDTO>(validationResults.Error);

            var eventToUpdate = await DbContext.ApplicationEvents.SingleAsync(x => x.Id == request.ApplicationEventDto.Id, cancellationToken);
            Mapper.Map(request.ApplicationEventDto, eventToUpdate);

            await DbContext.SaveChangesAsync(cancellationToken);

            return Result.Ok(request.ApplicationEventDto);
        }

        protected override Result CustomValidate(UpdateApplicationEventCommand request) =>
            DbContext.ApplicationEvents
                .Any(x => x.Id == request.ApplicationEventDto.Id)
                ? Result.Ok()
                : Result.Fail(Constants.ErrorMessages.NoEventExists);
    }
}
