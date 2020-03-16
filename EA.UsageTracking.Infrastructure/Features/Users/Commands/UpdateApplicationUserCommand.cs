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
using EA.UsageTracking.SharedKernel.Functional;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EA.UsageTracking.Infrastructure.Features.Users.Commands
{

    public class UpdateApplicationUserCommand : IRequest<Result<ApplicationUserDTO>>
    {
        public ApplicationUserDTO ApplicationUserDTO { get; set; }
    }

    public class UpdateApplicationUserCommandHandler : AsyncBaseHandler<UpdateApplicationUserCommand>, IRequestHandler<UpdateApplicationUserCommand, Result<ApplicationUserDTO>>
    {
        public UpdateApplicationUserCommandHandler(IUsageTrackingContextFactory usageTrackingContextFactory, IMapper mapper) :
            base(usageTrackingContextFactory, mapper)
        { }

        public async Task<Result<ApplicationUserDTO>> Handle(UpdateApplicationUserCommand request, CancellationToken cancellationToken)
        {
            var validationResults = Validate(request);
            if (validationResults.IsFailure)
                return Result.Fail<ApplicationUserDTO>(validationResults.Error);

            var userToUpdate =
                await DbContext.ApplicationUsers.SingleAsync(u => u.Id == request.ApplicationUserDTO.Id,
                    cancellationToken);
            Mapper.Map(request.ApplicationUserDTO, userToUpdate);

            await DbContext.SaveChangesAsync(cancellationToken);

            return Result.Ok(request.ApplicationUserDTO);
        }

        protected override Result CustomValidate(UpdateApplicationUserCommand request) =>
            DbContext.ApplicationUsers
                .Any(x => x.Id == request.ApplicationUserDTO.Id) ? Result.Ok() : Result.Fail(Constants.ErrorMessages.NoUserExists);
    }
}
