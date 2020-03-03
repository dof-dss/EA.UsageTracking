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
using EA.UsageTracking.Infrastructure.Features.Events.Commands;
using EA.UsageTracking.SharedKernel;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Extensions;
using MediatR;

namespace EA.UsageTracking.Infrastructure.Features.Users.Commands
{

    public class UpdateApplicationUserCommand : IRequest<Result<ApplicationUserDTO>>
    {
        public ApplicationUserDTO ApplicationUserDTO { get; set; }
    }

    public class UpdateApplicationUserCommandHandler : AsyncBaseHandler, IRequestHandler<UpdateApplicationUserCommand, Result<ApplicationUserDTO>>
    {
        public UpdateApplicationUserCommandHandler(IUsageTrackingContextFactory usageTrackingContextFactory, IMapper mapper) :
            base(usageTrackingContextFactory, mapper)
        { }

        public async Task<Result<ApplicationUserDTO>> Handle(UpdateApplicationUserCommand request, CancellationToken cancellationToken)
        {
            var applicationResult = DbContext.Applications.SingleOrDefault()
                .ToMaybe().ToResult(Constants.ErrorMessages.NoTenantExists);

            var applicationUserResult = DbContext.ApplicationUsers
                .SingleOrDefault(x => x.Id == request.ApplicationUserDTO.Id)
                .ToMaybe().ToResult(Constants.ErrorMessages.NoUserExists);

            var combinedResults = Result.Combine(applicationResult, applicationUserResult);
            if (combinedResults.IsFailure)
                return Result.Fail<ApplicationUserDTO>(combinedResults.Error);

            Mapper.Map(request.ApplicationUserDTO, applicationUserResult.Value);

            await DbContext.SaveChangesAsync();

            return Result.Ok(request.ApplicationUserDTO);
        }
    }
}
