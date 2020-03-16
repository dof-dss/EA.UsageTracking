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
using EA.UsageTracking.Infrastructure.Features.Events.Commands;
using EA.UsageTracking.Infrastructure.Features.Users.Validation;
using EA.UsageTracking.SharedKernel;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Extensions;
using EA.UsageTracking.SharedKernel.Functional;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EA.UsageTracking.Infrastructure.Features.Users.Commands
{
    public class AddApplicationUserCommand : IRequest<Result<ApplicationUserDTO>>
    {
        public ApplicationUserDTO ApplicationUserDto { get; set; }
    }

    public class AddApplicationUserCommandHandler : AsyncBaseHandler<AddApplicationUserCommand>, IRequestHandler<AddApplicationUserCommand, Result<ApplicationUserDTO>>
    {
        private ApplicationUser _applicationUser;
        private readonly AddApplicationUserCommandValidator _validator;

        public AddApplicationUserCommandHandler(IUsageTrackingContextFactory usageTrackingContextFactory, IMapper mapper):
            base(usageTrackingContextFactory, mapper)
        {
            _validator = new AddApplicationUserCommandValidator();
        }

        /// <summary>
        /// If user exists then associate application to them, if not already associated.
        /// If user does not exist, create and associate application to them.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Result<ApplicationUserDTO>> Handle(AddApplicationUserCommand request, CancellationToken cancellationToken)
        {
            var validationResults = Validate(request);
            if (validationResults.IsFailure)
                return Result.Fail<ApplicationUserDTO>(validationResults.Error);

            var application = DbContext.Applications.Single();
            _applicationUser = Mapper.Map<ApplicationUser>(request.ApplicationUserDto);

            DbContext.ApplicationUsers
                .Include(x => x.UserToApplications)
                .SingleOrDefault(u => u.Id == _applicationUser.Id)
                .ToMaybe()
                .Match(au => AssociateAppToUser(au, application),  
                    () => AddUserAndAssociateApp(application));

            await DbContext.SaveChangesAsync(cancellationToken);

            return Result.Ok(Mapper.Map<ApplicationUserDTO>(_applicationUser));
        }

        private void AddUserAndAssociateApp(Application application)
        {
            _applicationUser.UserToApplications.Add(new UserToApplication
                { User = _applicationUser, Application =  application });

            DbContext.ApplicationUsers.Add(_applicationUser);
        }

        private void AssociateAppToUser(ApplicationUser au, Application application)
        {
            if (au.UserToApplications.All(x => x.ApplicationId != application.Id))
                    au.UserToApplications.Add(new UserToApplication
                        { User = au, Application = application });
            _applicationUser = au;
        }

        protected override Result CustomValidate(AddApplicationUserCommand request)
        {
            var validationResults = _validator.Validate(request);
            return !validationResults.IsValid ? Result.Fail(validationResults.ToString(",")) : Result.Ok();
        }
    }
}
