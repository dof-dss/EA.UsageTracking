using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
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
using Microsoft.Extensions.DependencyInjection;

namespace EA.UsageTracking.Infrastructure.Features.Users.Commands
{
    public class SaveApplicationUserSubscriberCommand : IRequest<Result<ApplicationUserDTO>>
    {
        public string TenantId { get; set; }
        public string IdentityToken { get; set; }
    }

    public class SaveApplicationUserSubscriberCommandHandler : IRequestHandler<SaveApplicationUserSubscriberCommand, Result<ApplicationUserDTO>>
    {
        private ApplicationUser _applicationUser;
        private readonly SaveApplicationUserSubscriberCommandValidator _validator;
        private readonly IMapper _mapper;
        private UsageTrackingContext _dbContext;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public SaveApplicationUserSubscriberCommandHandler(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
        {
            _validator = new SaveApplicationUserSubscriberCommandValidator();
            _mapper = mapper;
            _serviceScopeFactory = serviceScopeFactory;
        }

        /// <summary>
        /// If user exists then associate application to them, if not already associated.
        /// If user does not exist, create and associate application to them.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Result<ApplicationUserDTO>> Handle(SaveApplicationUserSubscriberCommand request, CancellationToken cancellationToken)
        {
            var validationResults = Validate(request);
            if (validationResults.IsFailure) return Result.Fail<ApplicationUserDTO>(validationResults.Error);

            var applicationUserDtoResult = request.IdentityToken.ParseIdentityToken();
            if (applicationUserDtoResult.IsFailure) return applicationUserDtoResult;

            using var scope = _serviceScopeFactory.CreateScope();
            var scopedServices = scope.ServiceProvider;
            _dbContext = scopedServices.GetRequiredService<UsageTrackingContext>();
            _dbContext.TenantId = request.TenantId;

            var application = _dbContext.Applications.Single();
            _applicationUser = _mapper.Map<ApplicationUser>(applicationUserDtoResult.Value);

            _dbContext.ApplicationUsers
                .Include(x => x.UserToApplications)
                .SingleOrDefault(u => u.Id == _applicationUser.Id)
                .ToMaybe()
                .Match(au => AssociateAppToUser(au, application),
                    () => AddUserAndAssociateApp(application));

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Ok(_mapper.Map<ApplicationUserDTO>(_applicationUser));
        }

        private void AddUserAndAssociateApp(Application application)
        {
            _applicationUser.UserToApplications.Add(new UserToApplication
                { User = _applicationUser, Application =  application });

            _dbContext.ApplicationUsers.Add(_applicationUser);
        }

        private void AssociateAppToUser(ApplicationUser au, Application application)
        {
            if (au.UserToApplications.All(x => x.ApplicationId != application.Id))
                    au.UserToApplications.Add(new UserToApplication
                        { User = au, Application = application });
            _applicationUser = au;
        }

        private Result Validate(SaveApplicationUserSubscriberCommand request)
        {
            var validationResults = _validator.Validate(request);
            return !validationResults.IsValid ? Result.Fail(validationResults.ToString(",")) : Result.Ok();
        }
    }
}
