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
using EA.UsageTracking.SharedKernel.Functional;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EA.UsageTracking.Infrastructure.Features.Users.Commands
{

    public class DeleteApplicationUserCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    public class DeleteApplicationUserCommandHandler : AsyncBaseHandler<DeleteApplicationUserCommand>, IRequestHandler<DeleteApplicationUserCommand, Result>
    {
        public DeleteApplicationUserCommandHandler(IUsageTrackingContextFactory usageTrackingContextFactory, IMapper mapper) :
            base(usageTrackingContextFactory, mapper)
        { }

        public async Task<Result> Handle(DeleteApplicationUserCommand request, CancellationToken cancellationToken)
        {
            var validationResults = Validate(request);
            if (validationResults.IsFailure)
                return Result.Fail(validationResults.Error);

            var applicationUserToRemove = DbContext.ApplicationUsers.Single(x => x.Id == request.Id);
            DbContext.ApplicationUsers.Remove(applicationUserToRemove);

            await DbContext.SaveChangesAsync();

            return Result.Ok();
        }

        protected override Result CustomValidate(DeleteApplicationUserCommand request) =>
            DbContext.ApplicationUsers
                .SingleOrDefault(x => x.Id == request.Id)
                .ToMaybe().ToResult(Constants.ErrorMessages.NoUserExists);
    }
}
