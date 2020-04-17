using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.SharedKernel.Extensions;
using EA.UsageTracking.SharedKernel.Functional;

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
    }
}
