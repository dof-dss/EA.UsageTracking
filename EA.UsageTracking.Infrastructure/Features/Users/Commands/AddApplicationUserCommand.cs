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
using EA.UsageTracking.Infrastructure.Features.Events.Commands;
using EA.UsageTracking.SharedKernel;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Extensions;
using MediatR;

namespace EA.UsageTracking.Infrastructure.Features.Users.Commands
{
    public class AddApplicationUserCommand : IRequest<Result<ApplicationUserDTO>>
    {
        public ApplicationUserDTO ApplicationUserDto { get; set; }
    }

    public class AddApplicationUserCommandHandler : IRequestHandler<AddApplicationUserCommand, Result<ApplicationUserDTO>>
    {
        private readonly UsageTrackingContext _dbContext;
        private readonly IMapper _mapper;

        public AddApplicationUserCommandHandler(IUsageTrackingContextFactory usageTrackingContextFactory, IMapper mapper)
        {
            _dbContext = usageTrackingContextFactory.UsageTrackingContext;
            _mapper = mapper;
        }

        public async Task<Result<ApplicationUserDTO>> Handle(AddApplicationUserCommand request, CancellationToken cancellationToken)
        {
            var applicationResult = _dbContext.Applications.SingleOrDefault().ToMaybe().ToResult(Constants.ErrorMessages.NoTenantExists);
            if (applicationResult.IsFailure)
                return Result.Fail<ApplicationUserDTO>(applicationResult.Error);

            var applicationUser = _mapper.Map<ApplicationUser>(request.ApplicationUserDto);
            applicationUser.UserToApplications.Add(new UserToApplication
                {User = applicationUser, Application = applicationResult.Value});

            _dbContext.ApplicationUsers.Add(applicationUser);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Ok(_mapper.Map<ApplicationUserDTO>(applicationUser));
        }
    }
}
