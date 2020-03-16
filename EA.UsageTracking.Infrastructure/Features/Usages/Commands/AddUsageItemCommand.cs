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
    public class AddUsageItemCommand : IRequest<Result<int>>
    { 
        public Guid TenantId { get; set; }
        public int ApplicationEventId { get; set; }
        public Guid ApplicationUserId { get; set; }
    }

    public class AddUsageItemCommandHandler : IRequestHandler<AddUsageItemCommand, Result<int>>
    {
        private readonly AddUsageItemCommandValidator _validator;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public AddUsageItemCommandHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _validator = new AddUsageItemCommandValidator();
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<Result<int>> Handle(AddUsageItemCommand request, CancellationToken cancellationToken)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var dbContext = scopedServices.GetRequiredService<UsageTrackingContext>();

                dbContext.TenantId = request.TenantId;

                var applicationResult = dbContext.Applications.AsNoTracking().SingleOrDefault().ToMaybe()
                    .ToResult(Constants.ErrorMessages.NoTenantExists);
                var validationResult = Validate(request);
                var combinedResults = Result.Combine(applicationResult, validationResult);
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

        protected Result Validate(AddUsageItemCommand request)
        {
            var validate = _validator.Validate(request);
            return !validate.IsValid ? Result.Fail(validate.ToString(",")) : Result.Ok();
        }
    }
}
