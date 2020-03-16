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
using EA.UsageTracking.Infrastructure.Features.Users.Validation;
using EA.UsageTracking.SharedKernel;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Extensions;
using EA.UsageTracking.SharedKernel.Functional;
using MediatR;

namespace EA.UsageTracking.Infrastructure.Features.Events.Commands
{
    public class DeleteApplicationEventCommand: IRequest<Result>
    {
        public int Id { get; set; }
    }

    public class DeleteApplicationEventCommandHandler : AsyncBaseHandler<DeleteApplicationEventCommand>, IRequestHandler<DeleteApplicationEventCommand, Result>
    {
        public DeleteApplicationEventCommandHandler(IUsageTrackingContextFactory usageTrackingContextFactory,
            IMapper mapper) :
            base(usageTrackingContextFactory, mapper)
        {}

        public async Task<Result> Handle(DeleteApplicationEventCommand request, CancellationToken cancellationToken)
        {
            var validateResults = Validate(request);
            if (validateResults.IsFailure)
                return Result.Fail(validateResults.Error);

            var applicationEvent = new ApplicationEvent {Id = request.Id};
            DbContext.ApplicationEvents.Remove(applicationEvent);

            await DbContext.SaveChangesAsync();

            return Result.Ok();
        }

        protected override Result CustomValidate(DeleteApplicationEventCommand request) => 
            DbContext.ApplicationEvents
                .Any(x => x.Id == request.Id) ? Result.Ok() : Result.Fail(Constants.ErrorMessages.NoEventExists);
    }
}
