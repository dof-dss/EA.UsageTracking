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
using MediatR;

namespace EA.UsageTracking.Infrastructure.Features.Events.Commands
{
    public class UpdateApplicationEventCommand: IRequest<Result<ApplicationEventDTO>>
    {
        public ApplicationEventDTO ApplicationEventDto { get; set; }
    }

    public class UpdateApplicationEventCommandHandler : AsyncBaseHandler, IRequestHandler<UpdateApplicationEventCommand,Result<ApplicationEventDTO>>
    {
        public UpdateApplicationEventCommandHandler(IUsageTrackingContextFactory usageTrackingContextFactory, IMapper mapper) :
            base(usageTrackingContextFactory, mapper)
        {}

        public async Task<Result<ApplicationEventDTO>> Handle(UpdateApplicationEventCommand request, CancellationToken cancellationToken)
        {
            var applicationResult = DbContext.Applications.SingleOrDefault()
                .ToMaybe().ToResult(Constants.ErrorMessages.NoTenantExists);

            var applicationEventResult = DbContext.ApplicationEvents
                .SingleOrDefault(x => x.Id == request.ApplicationEventDto.Id)
                .ToMaybe().ToResult(Constants.ErrorMessages.NoEventExists);

            var combinedResults = Result.Combine(applicationResult, applicationEventResult);
            if (combinedResults.IsFailure)
                return Result.Fail<ApplicationEventDTO>(combinedResults.Error);

            Mapper.Map(request.ApplicationEventDto, applicationEventResult.Value);

            await DbContext.SaveChangesAsync();

            return Result.Ok(request.ApplicationEventDto);
        }
    }
}
