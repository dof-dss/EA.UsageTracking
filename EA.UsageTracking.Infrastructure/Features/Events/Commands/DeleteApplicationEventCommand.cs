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
    public class DeleteApplicationEventCommand: IRequest<Result>
    {
        public int Id { get; set; }
    }

    public class DeleteApplicationEventCommandHandler : AsyncBaseHandler, IRequestHandler<DeleteApplicationEventCommand, Result>
    {
        public DeleteApplicationEventCommandHandler(IUsageTrackingContextFactory usageTrackingContextFactory, IMapper mapper) :
            base(usageTrackingContextFactory, mapper)
        {}

        public async Task<Result> Handle(DeleteApplicationEventCommand request, CancellationToken cancellationToken)
        {
            var applicationResult = DbContext.Applications.SingleOrDefault()
                .ToMaybe().ToResult(Constants.ErrorMessages.NoTenantExists);

            var applicationEventResult = DbContext.ApplicationEvents
                .SingleOrDefault(x => x.Id == request.Id)
                .ToMaybe().ToResult(Constants.ErrorMessages.NoEventExists);

            var combinedResults = Result.Combine(applicationResult, applicationEventResult);
            if (combinedResults.IsFailure)
                return Result.Fail(combinedResults.Error);

            DbContext.ApplicationEvents.Remove(applicationEventResult.Value);

            await DbContext.SaveChangesAsync();

            return Result.Ok();
        }
    }
}
