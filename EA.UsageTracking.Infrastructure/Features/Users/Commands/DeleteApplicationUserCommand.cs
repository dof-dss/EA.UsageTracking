using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.Infrastructure.Features.Common;
using EA.UsageTracking.Infrastructure.Features.Events.Commands;
using EA.UsageTracking.SharedKernel;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Extensions;
using MediatR;

namespace EA.UsageTracking.Infrastructure.Features.Users.Commands
{

    public class DeleteApplicationUserCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    public class DeleteApplicationUserCommandHandler : AsyncBaseHandler, IRequestHandler<DeleteApplicationUserCommand, Result>
    {
        public DeleteApplicationUserCommandHandler(IUsageTrackingContextFactory usageTrackingContextFactory, IMapper mapper) :
            base(usageTrackingContextFactory, mapper)
        { }

        public async Task<Result> Handle(DeleteApplicationUserCommand request, CancellationToken cancellationToken)
        {
            var applicationResult = DbContext.Applications.SingleOrDefault()
                .ToMaybe().ToResult(Constants.ErrorMessages.NoTenantExists);

            var applicationUserResult = DbContext.ApplicationUsers
                .SingleOrDefault(x => x.Id == request.Id)
                .ToMaybe().ToResult(Constants.ErrorMessages.NoUserExists);

            var combinedResults = Result.Combine(applicationResult, applicationUserResult);
            if (combinedResults.IsFailure)
                return Result.Fail(combinedResults.Error);

            DbContext.ApplicationUsers.Remove(applicationUserResult.Value);

            await DbContext.SaveChangesAsync();

            return Result.Ok();
        }
    }
}
