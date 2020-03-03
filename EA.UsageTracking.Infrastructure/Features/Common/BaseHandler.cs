using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using EA.UsageTracking.Infrastructure.Data;
using MediatR;

namespace EA.UsageTracking.Infrastructure.Features.Common
{
    public class AsyncBaseHandler
    {
        protected readonly UsageTrackingContext DbContext;
        protected readonly IMapper Mapper;

        public AsyncBaseHandler(IUsageTrackingContextFactory usageTrackingContextFactory, IMapper mapper)
        {
            DbContext = usageTrackingContextFactory.UsageTrackingContext;
            Mapper = mapper;
        }
    }

    public abstract class BaseHandler<TRequest, TResponse>: RequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        protected readonly UsageTrackingContext DbContext;
        protected readonly IMapper Mapper;

        protected BaseHandler(IUsageTrackingContextFactory usageTrackingContextFactory, IMapper mapper)
        {
            DbContext = usageTrackingContextFactory.UsageTrackingContext;
            Mapper = mapper;
        }
    }
}
