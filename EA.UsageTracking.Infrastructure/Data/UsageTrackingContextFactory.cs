using System;
using EA.UsageTracking.SharedKernel.Constants;
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

        private Guid TenantId
        {
            get
            {
                ValidateHttpContext();

                var tenantId = this._httpContext.Request.Headers[Constants.Tenant.TenantId].ToString();

                return MarshallTenantId(tenantId);
            }
        }

        private void ValidateHttpContext()
        {
            if (this._httpContext == null)
            {
                throw new ArgumentNullException(nameof(this._httpContext));
            }
        }

        private static Guid MarshallTenantId(string tenantId)
        {
            if (tenantId == null)
            {
                throw new ArgumentNullException(nameof(tenantId));
            }

            if (!Guid.TryParse(tenantId, out Guid tenantGuid))
            {
                throw new ArgumentNullException(nameof(tenantId));
            }

            return tenantGuid;
        }
    }
}
