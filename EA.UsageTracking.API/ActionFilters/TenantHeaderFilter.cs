using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EA.UsageTracking.SharedKernel.Constants;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EA.UsageTracking.API.ActionFilters
{
    public class TenantHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = Constants.Tenant.TenantId,
                In = ParameterLocation.Header,
                Description = Constants.Tenant.TenantIdSwaggerDescription,
                Required = true
            });
        }
    }
}
