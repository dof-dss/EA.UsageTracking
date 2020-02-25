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

    public class GetUsagesForUserQuery : IRequest<Result<List<UsageItemDTO>>>
    {
        public int ApplicationId { get; set; }
        public int UserId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class GetUsagesForUserQueryHandler : RequestHandler<GetUsagesForUserQuery, Result<List<UsageItemDTO>>>
    {
        private readonly UsageTrackingContext _dbContext;

        public GetUsagesForUserQueryHandler(UsageTrackingContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override Result<List<UsageItemDTO>> Handle(GetUsagesForUserQuery message)
        {
            if (message.PageNumber < 1 || message.PageSize < 1)
                return Result.Fail<List<UsageItemDTO>>("Incorrect pagination values");

            var results = _dbContext.UsageItems
                .AsNoTracking()
                .Include(a => a.Application)
                .Include(e => e.ApplicationEvent)
                .Include(u => u.ApplicationUser)
                .OrderBy(x => x.Id).ThenBy(y => y.ApplicationUser.Id).ThenBy(z => z.ApplicationEvent.Id)
                .Skip((message.PageNumber - 1) * message.PageSize)
                .Take(message.PageSize)
                .Where(i => i.Application.Id == message.ApplicationId && i.ApplicationUser.Id == message.UserId);

            return Result.Ok(results.Select(i => UsageItemDTO.FromUsageItem(i)).ToList());
        }

    }
}
