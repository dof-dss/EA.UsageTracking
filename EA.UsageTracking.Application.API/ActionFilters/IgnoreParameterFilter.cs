using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EA.UsageTracking.Application.API.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EA.UsageTracking.Application.API.ActionFilters
{
    public class IgnoreParameterFilter: IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (!AttributeApplied(context)) return;

            var attribute =
                context.ApiDescription.ActionDescriptor.EndpointMetadata.Single(x => x is IgnoreParameterAttribute) as IgnoreParameterAttribute;
            var parameterToRemove =
                operation.Parameters.SingleOrDefault(x => x.Name == attribute.ParameterToIgnore);
            operation.Parameters.Remove(parameterToRemove);
        }

        private bool AttributeApplied(OperationFilterContext context) =>
            context.ApiDescription.ActionDescriptor.EndpointMetadata.Any(x => x is IgnoreParameterAttribute);
    }
}
