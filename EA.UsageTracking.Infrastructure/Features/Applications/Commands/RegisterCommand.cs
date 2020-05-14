using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.Infrastructure.Features.Common;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Extensions;
using EA.UsageTracking.SharedKernel.Functional;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EA.UsageTracking.Infrastructure.Features.Applications.Commands
{
    public class RegisterCommand : IRequest<Result>
    {
        public int ApplicationId { get; set; }
        public string IdentityToken { get; set; }
    }

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result>
    {
        private readonly IMapper _mapper;
        private readonly UsageTrackingContext _usageTrackingContext;
        private readonly HttpContext _httpContext;

        public RegisterCommandHandler(UsageTrackingContext usageTrackingContext, IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _usageTrackingContext = usageTrackingContext;
            _mapper = mapper;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userIdResult = _httpContext.GetUserId();
                if (userIdResult.IsFailure) return Result.Fail(userIdResult.Error);
                var userId = userIdResult.Value;

                var applicationResult = _usageTrackingContext.Applications
                    .IgnoreQueryFilters()
                    .Include(ua => ua.UserToApplications)
                    .SingleOrDefault(a => a.Id == request.ApplicationId)
                    .ToMaybe().ToResult(Constants.ErrorMessages.NoTenant);
                if (applicationResult.IsFailure) return Result.Fail(applicationResult.Error);

                _usageTrackingContext.ApplicationUsers.SingleOrDefault(u => u.Id == userId)
                    .ToMaybe()
                    .Match(
                        u => u.AssociateUserToApp(applicationResult.Value),
                        () => AddUserAndAssociateToApp(request, applicationResult.Value));

                await _usageTrackingContext.SaveChangesAsync(cancellationToken);

                return Result.Ok();
            }
            catch (ArgumentException ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        private void AddUserAndAssociateToApp(RegisterCommand request, Application application)
        {
            var newUserResult = request.IdentityToken.ParseIdentityToken();
            if (newUserResult.IsFailure) throw new ArgumentException(Constants.ErrorMessages.NoUserExists);

            var newUser = _mapper.Map<ApplicationUser>(newUserResult.Value);

            _usageTrackingContext.ApplicationUsers.Add(newUser);

            application.UserToApplications.Add(new UserToApplication
            {
                User = newUser,
                Application = application
            });
        }
    }
}