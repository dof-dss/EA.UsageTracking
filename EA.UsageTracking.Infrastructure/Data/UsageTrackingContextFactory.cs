using System;
using System.Linq;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EA.UsageTracking.Infrastructure.Data
{
    public class UsageTrackingContextFactory : IUsageTrackingContextFactory
    {
        private readonly HttpContext _httpContext;
        private DbContextOptions<UsageTrackingContext> _options;

        public UsageTrackingContextFactory(IHttpContextAccessor httpContentAccessor,
            DbContextOptions<UsageTrackingContext> options)
        {
            _httpContext = httpContentAccessor.HttpContext;
            _options = options;
        }

        public UsageTrackingContext UsageTrackingContext => new UsageTrackingContext(_options, TenantId);

        private string TenantId
        {
            get
            {
                ValidateHttpContext();

                var maybeTenant = _httpContext.User.Claims.FirstOrDefault(c => c.Type == "client_id")
                    .ToMaybe()
                    .ValueOrThrow(new ArgumentNullException("client_id"));

                return maybeTenant.Value;
            }
        }

        private void ValidateHttpContext()
        {
            if (this._httpContext == null)
            {
                throw new ArgumentNullException(nameof(this._httpContext));
            }
        }
    }
}
