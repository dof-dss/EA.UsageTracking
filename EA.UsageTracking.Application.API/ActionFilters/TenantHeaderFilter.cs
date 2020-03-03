using System;
using System.Collections.Generic;
using System.Linq;
using EA.UsageTracking.Application.API.Attributes;
using EA.UsageTracking.Application.API.Controllers;
using EA.UsageTracking.SharedKernel.Constants;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EA.UsageTracking.Application.API.ActionFilters
{
    public class TenantHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (IgnoreTenantAttribute(context))
                return;

            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = Constants.Tenant.TenantId,
                In = ParameterLocation.Header,
                Description = Constants.Tenant.TenantIdSwaggerDescription,
                Required = true
            });
        }

        private bool IgnoreTenantAttribute(OperationFilterContext context) => 
            context.ApiDescription.ActionDescriptor.EndpointMetadata.Any(x => x is IgnoreTenantAttribute);
    }
}
