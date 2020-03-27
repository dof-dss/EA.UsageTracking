using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Extensions;
using EA.UsageTracking.SharedKernel.Functional;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace EA.UsageTracking.Infrastructure.Features.Common
{
    public abstract class AsyncBaseHandler<T>
    {
        protected readonly UsageTrackingContext DbContext;
        protected readonly IMapper Mapper;
        protected Maybe<Application> MaybeApplication = Maybe<Application>.None;
        protected readonly IDistributedCache Cache;

        protected AsyncBaseHandler(IUsageTrackingContextFactory usageTrackingContextFactory)
        {
            DbContext = usageTrackingContextFactory.UsageTrackingContext;
        }

        protected AsyncBaseHandler(IUsageTrackingContextFactory usageTrackingContextFactory, IMapper mapper)
        {
            DbContext = usageTrackingContextFactory.UsageTrackingContext;
            Mapper = mapper;
        }

        protected AsyncBaseHandler(UsageTrackingContext usageTrackingContext, IMapper mapper)
        {
            DbContext = usageTrackingContext;
            Mapper = mapper;
        }

        protected AsyncBaseHandler(IUsageTrackingContextFactory usageTrackingContextFactory, IMapper mapper, IDistributedCache cache)
        {
            DbContext = usageTrackingContextFactory.UsageTrackingContext;
            Mapper = mapper;
            Cache = cache;
        }

        protected Result Validate(T request)
        {
            MaybeApplication = DbContext.Applications.AsNoTracking().SingleOrDefault().ToMaybe();

            var applicationResult = MaybeApplication.ToResult(Constants.ErrorMessages.NoTenantExists);
            return applicationResult.IsFailure ? applicationResult : CustomValidate(request);
        }

        protected abstract Result CustomValidate(T request);
    }
}