using System.Linq;
using System.Threading.Tasks;
using EA.UsageTracking.Application.API.Authorization;
using EA.UsageTracking.SharedKernel.Constants;
using Microsoft.AspNetCore.Authorization;

namespace EA.UsageTracking.Tests.Functional
{
    public class HasScopeTestHandler: HasScopeHandler
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}