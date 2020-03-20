using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.Infrastructure.Features.Common;
using EA.UsageTracking.Infrastructure.Features.Events.Commands;
using EA.UsageTracking.Infrastructure.Features.Pagination;
using EA.UsageTracking.Infrastructure.Features.Usages.Validation;
using EA.UsageTracking.SharedKernel;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Extensions;
using EA.UsageTracking.SharedKernel.Functional;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace EA.UsageTracking.Infrastructure.Features.Usages.Commands
{
    public class AddUsageItemPublisherCommand : IRequest<Result>
    { 
        public int ApplicationEventId { get; set; }
        public Guid ApplicationUserId { get; set; }
    }

    public class AddUsageItemPublisherCommandHandler: IRequestHandler<AddUsageItemPublisherCommand, Result>
    {
        private readonly AddUsageItemCommandValidator _validator;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly HttpContext _httpContext;

        public AddUsageItemPublisherCommandHandler(IConnectionMultiplexer connectionMultiplexer, 
            IHttpContextAccessor httpContextAccessor)
        {
            _validator = new AddUsageItemCommandValidator();

            _connectionMultiplexer = connectionMultiplexer;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<Result> Handle(AddUsageItemPublisherCommand request, CancellationToken cancellationToken)
        {
            var subscriberCommand = new AddUsageItemSubscriberCommand
            {
                TenantId = Guid.Parse(_httpContext.Request.Headers[Constants.Tenant.TenantId].ToString()),
                RequestId = Guid.NewGuid(),
                ApplicationEventId = request.ApplicationEventId,
                ApplicationUserId = request.ApplicationUserId
            };

            var validationResults = Validate(subscriberCommand);
            if(validationResults.IsFailure) return Result.Fail(validationResults.Error);

            var publisher = _connectionMultiplexer.GetSubscriber();
            await publisher.PublishAsync("Usage", JsonConvert.SerializeObject(subscriberCommand), CommandFlags.FireAndForget);
            return Result.Ok();
        }

        protected Result Validate(AddUsageItemSubscriberCommand request)
        {
            var validate = _validator.Validate(request);
            return !validate.IsValid ? Result.Fail(validate.ToString(",")) : Result.Ok();
        }
    }
}
