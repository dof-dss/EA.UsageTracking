using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Features.Pagination;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Extensions;
using EA.UsageTracking.SharedKernel.Functional;
using Microsoft.AspNetCore.Http;

namespace EA.UsageTracking.Infrastructure.Features.Common
{
    public static class HelperExtensions
    {
        public static Result<ApplicationUserDTO> ParseIdentityToken(this string identityToken)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtSecurityTokenHandler.ReadJwtToken(identityToken);

            var subResult = token.Claims.FirstOrDefault(c => c.Type == "sub").ToMaybe().ToResult("No sub claim");
            var emailResult = token.Claims.FirstOrDefault(c => c.Type == "email").ToMaybe().ToResult("No email claim");
            var usernameResult = token.Claims.FirstOrDefault(c => c.Type == "cognito:username").ToMaybe().ToResult("No username claim");
            var combinedResults = Result.Combine(subResult, emailResult, usernameResult);
            if (combinedResults.IsFailure) return Result.Fail<ApplicationUserDTO>(combinedResults.Error);

            if (!Guid.TryParse(subResult.Value.Value, out var applicationUserId))
                return Result.Fail<ApplicationUserDTO>("Invalid GUID");

            var applicationUserDto = new ApplicationUserDTO
            {
                Id = applicationUserId,
                Email = emailResult.Value.Value,
                Name = usernameResult.Value.Value,
            };

            return Result.Ok(applicationUserDto);
        }

        public static Result<Guid> GetUserId(this HttpContext httpContext)
        {
            var subResult = httpContext.User.Claims.FirstOrDefault(c => c.Type == "username")
                .ToMaybe().ToResult(Constants.ErrorMessages.InvalidClaim);
            if (subResult.IsFailure)
                return Result.Fail<Guid>(subResult.Error);

            return !Guid.TryParse(subResult.Value.Value, out var userId) 
                ? Result.Fail<Guid>(Constants.ErrorMessages.InvalidGuid) 
                : Result.Ok(userId);
        }

        public static void AssociateUserToApp(this ApplicationUser applicationUser, Application application)
        {
            application.UserToApplications.Add(new UserToApplication
            {
                User = applicationUser,
                Application = application
            });
        }
    }
}
