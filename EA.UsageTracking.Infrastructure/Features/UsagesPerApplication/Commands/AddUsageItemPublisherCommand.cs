using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EA.UsageTracking.Infrastructure.Features.UsagesPerApplication.Validation;
using EA.UsageTracking.SharedKernel.Extensions;
using EA.UsageTracking.SharedKernel.Functional;
using MediatR;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace EA.UsageTracking.Infrastructure.Features.UsagesPerApplication.Commands
{
    public class AddUsageItemPublisherCommand : IRequest<Result>
    { 
        public int ApplicationEventId { get; set; }
        public string IdentityToken { get; set; }
    }

    public class AddUsageItemPublisherCommandHandler: IRequestHandler<AddUsageItemPublisherCommand, Result>
    {
        private readonly AddUsageItemCommandValidator _validator;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly HttpContext _httpContext;
        public string TenantId
        {
            get
            {
                var maybeClaim = _httpContext.User.Claims.FirstOrDefault(c => c.Type == "client_id")
                    .ToMaybe()
                    .ValueOrThrow(new ArgumentNullException("client_id"));

                return maybeClaim.Value;
            }
        }

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
                TenantId = TenantId,
                RequestId = Guid.NewGuid(),
                ApplicationEventId = request.ApplicationEventId,
                IdentityToken = request.IdentityToken
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
