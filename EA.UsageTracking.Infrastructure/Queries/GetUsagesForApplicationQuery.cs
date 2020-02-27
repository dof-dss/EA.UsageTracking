using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Core.Queries;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.SharedKernel;
using EA.UsageTracking.SharedKernel.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EA.UsageTracking.Infrastructure.Queries
{

    public class GetUsagesForApplicationQuery : IRequest<Result<List<UsageItemDTO>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;
    }

    public class GetUsagesForApplicationQueryHandler : RequestHandler<GetUsagesForApplicationQuery, Result<List<UsageItemDTO>>>
    {
        private readonly UsageTrackingContext _dbContext;

        public GetUsagesForApplicationQueryHandler(IUsageTrackingContextFactory dbContextFactory)
        {
            _dbContext = dbContextFactory.UsageTrackingContext;
        }

        protected override Result<List<UsageItemDTO>> Handle(GetUsagesForApplicationQuery message)
        {
            if (message.PageNumber < 1 || message.PageSize < 1)
                return Result.Fail<List<UsageItemDTO>>("Incorrect pagination values");

            var results = _dbContext.UsageItems
                .AsNoTracking()
                .Include(a => a.Application)
                .Include(e => e.ApplicationEvent)
                .Include(u => u.ApplicationUser)
                .OrderBy(u => u.Id).ThenBy(y => y.ApplicationUser.Id)
                .Skip((message.PageNumber - 1) * message.PageSize)
                .Take(message.PageSize);

            return Result.Ok(results.Select(i => UsageItemDTO.FromUsageItem(i)).ToList());
        }

    }
}
