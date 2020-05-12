using System;
using System.Linq;
using System.Threading.Tasks;
using EA.UsageTracking.SharedKernel.Constants;
using Microsoft.AspNetCore.Authorization;

namespace EA.UsageTracking.Application.API.Authorization
{
    public class HasScopeHandler : AuthorizationHandler<HasScopeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == "scope" && c.Issuer == requirement.Issuer))
                return Task.CompletedTask;

            var scopes = context.User.FindFirst(c => c.Type == "scope" && c.Issuer == requirement.Issuer).Value.Split(' ');

            if (scopes.Any(s => s == Constants.Policy.UsageAdmin || s == requirement.Scope))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}