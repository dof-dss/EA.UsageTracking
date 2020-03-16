using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using AutoMapper;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Extensions;
using EA.UsageTracking.SharedKernel.Functional;
using MediatR;

namespace EA.UsageTracking.Infrastructure.Features.Common
{
    public abstract class BaseHandler<TRequest, TResponse>: RequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        protected readonly UsageTrackingContext DbContext;
        protected readonly IMapper Mapper;

        protected BaseHandler(IUsageTrackingContextFactory usageTrackingContextFactory, IMapper mapper)
        {
            DbContext = usageTrackingContextFactory.UsageTrackingContext;
            Mapper = mapper;
        }

        protected Result Validate(TRequest request)
        {
            var applicationResult = DbContext.Applications.Any()
                ? Result.Ok()
                : Result.Fail(Constants.ErrorMessages.NoTenantExists);

            var customResult = CustomValidate(request);

            return Result.Combine(applicationResult, customResult);
        }

        protected abstract Result CustomValidate(TRequest request);
    }
}
