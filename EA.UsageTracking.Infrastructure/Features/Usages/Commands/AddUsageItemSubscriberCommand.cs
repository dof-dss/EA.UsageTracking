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
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace EA.UsageTracking.Infrastructure.Features.Usages.Commands
{
    public class AddUsageItemSubscriberCommand : IRequest<Result<int>>
    { 
        public Guid TenantId { get; set; }
        public Guid RequestId { get; set; }
        public int ApplicationEventId { get; set; }
        public Guid ApplicationUserId { get; set; }
    }

    public class AddUsageItemSubscriberCommandHandler : IRequestHandler<AddUsageItemSubscriberCommand, Result<int>>
    {
        private readonly AddUsageItemCommandValidator _validator;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public AddUsageItemSubscriberCommandHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _validator = new AddUsageItemCommandValidator();
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<Result<int>> Handle(AddUsageItemSubscriberCommand request, CancellationToken cancellationToken)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var dbContext = scopedServices.GetRequiredService<UsageTrackingContext>();

                dbContext.TenantId = request.TenantId;

                var applicationResult = dbContext.Applications.AsNoTracking().SingleOrDefault().ToMaybe()
                    .ToResult(Constants.ErrorMessages.NoTenantExists);
                var userResult = dbContext.ApplicationUsers.AsNoTracking().SingleOrDefault(u => u.Id == request.ApplicationUserId).ToMaybe()
                    .ToResult(Constants.ErrorMessages.NoUserExists);
                var eventResult = dbContext.ApplicationEvents.AsNoTracking().SingleOrDefault(e => e.Id == request.ApplicationEventId).ToMaybe()
                    .ToResult(Constants.ErrorMessages.NoEventExists);

                var validationResult = Validate(request);
                var combinedResults = Result.Combine(applicationResult, userResult, eventResult, validationResult);
                if(combinedResults.IsFailure) return Result.Fail<int>(combinedResults.Error);

                var usageItem = new UsageItem()
                {
                    ApplicationId = applicationResult.Value.Id,
                    ApplicationEventId = request.ApplicationEventId,
                    ApplicationUserId = request.ApplicationUserId,
                };

                dbContext.UsageItems.Add(usageItem);
                await dbContext.SaveChangesAsync(cancellationToken);

                return Result.Ok(usageItem.Id);
            }
        }

        protected Result Validate(AddUsageItemSubscriberCommand request)
        {
            var validate = _validator.Validate(request);
            return !validate.IsValid ? Result.Fail(validate.ToString(",")) : Result.Ok();
        }
    }
}
