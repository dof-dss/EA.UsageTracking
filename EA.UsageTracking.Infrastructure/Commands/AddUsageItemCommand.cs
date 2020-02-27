using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Core.Queries;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.SharedKernel;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EA.UsageTracking.Infrastructure.Commands
{
    public class AddUsageItemCommand : IRequest<Result<UsageItemDTO>>
    {
        public UsageItemDTO UsageItemDTO { get; set; }
    }

    public class AddUsageItemCommandHandler : IRequestHandler<AddUsageItemCommand, Result<UsageItemDTO>>
    {
        private readonly UsageTrackingContext _dbContext;

        public AddUsageItemCommandHandler(IUsageTrackingContextFactory dbContextFactory)
        {
            _dbContext = dbContextFactory.UsageTrackingContext;
        }

        public async Task<Result<UsageItemDTO>> Handle(AddUsageItemCommand request, CancellationToken cancellationToken)
        {
            var applicationResult = _dbContext.Applications.SingleOrDefault(x => x.Id == request.UsageItemDTO.ApplicationId).ToMaybe()
                .ToResult(Constants.ErrorMessages.NoApplicationExists);
            var applicationEventResult = _dbContext.ApplicationEvents.SingleOrDefault(x => x.Id == request.UsageItemDTO.ApplicationEventId).ToMaybe()
                .ToResult(Constants.ErrorMessages.NoEventExists);
            var applicationUserResult = _dbContext.ApplicationUsers.SingleOrDefault(x => x.Id == request.UsageItemDTO.ApplicationUserId).ToMaybe()
                .ToResult(Constants.ErrorMessages.NoUserExists);

            var combinedResult = Result.Combine(applicationResult, applicationEventResult, applicationUserResult);
            if (combinedResult.IsFailure)
                return Result.Fail<UsageItemDTO>(combinedResult.Error);

            var usageItem = new UsageItem()
            {
                Application = applicationResult.Value,
                ApplicationEvent = applicationEventResult.Value,
                ApplicationUser = applicationUserResult.Value,
            };

            _dbContext.UsageItems.Add(usageItem);
            await _dbContext.SaveChangesAsync();

            return Result.Ok(UsageItemDTO.FromUsageItem(usageItem));
        }
    }
}
