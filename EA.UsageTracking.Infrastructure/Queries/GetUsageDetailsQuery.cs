using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.SharedKernel;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EA.UsageTracking.Core.Queries
{

    public class GetUsageDetailsQuery : IRequest<Result<UsageItemDTO>>
    {
        public int Id { get; set; }
    }

    public class GetUsageDetailsQueryHandler : IRequestHandler<GetUsageDetailsQuery, Result<UsageItemDTO>>
    {
        private readonly UsageTrackingContext _dbContext;

        public GetUsageDetailsQueryHandler(UsageTrackingContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<UsageItemDTO>> Handle(GetUsageDetailsQuery request, CancellationToken cancellationToken)
        {
            var result = await _dbContext.UsageItems
                .AsNoTracking()
                .Include(a => a.Application)
                .Include(au => au.ApplicationUser)
                .Include(ae => ae.ApplicationEvent)
                .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

                return result.ToMaybe().ToResult(Constants.ErrorMessages.NoItemExists)
                .OnBoth(i => i.IsSuccess
                    ? Result.Ok(UsageItemDTO.FromUsageItem(i.Value))
                    : Result.Fail<UsageItemDTO>(i.Error));
        }
    }
}
