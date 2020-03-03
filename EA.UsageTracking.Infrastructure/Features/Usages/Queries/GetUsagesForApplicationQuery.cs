using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.Infrastructure.Features.Common;
using EA.UsageTracking.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EA.UsageTracking.Infrastructure.Features.Usages.Queries
{

    public class GetUsagesForApplicationQuery : IRequest<Result<List<UsageItemDTO>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;
    }

    public class GetUsagesForApplicationQueryHandler : BaseHandler<GetUsagesForApplicationQuery, Result<List<UsageItemDTO>>>
    {
        public GetUsagesForApplicationQueryHandler(IUsageTrackingContextFactory dbContextFactory, IMapper mapper)
        : base(dbContextFactory, mapper)
        {}

        protected override Result<List<UsageItemDTO>> Handle(GetUsagesForApplicationQuery message)
        {
            if (message.PageNumber < 1 || message.PageSize < 1)
                return Result.Fail<List<UsageItemDTO>>("Incorrect pagination values");

            var results = DbContext.UsageItems
                .AsNoTracking()
                .Include(a => a.Application)
                .Include(e => e.ApplicationEvent)
                .Include(u => u.ApplicationUser)
                .OrderBy(u => u.Id).ThenBy(y => y.ApplicationUser.Id)
                .Skip((message.PageNumber - 1) * message.PageSize)
                .Take(message.PageSize);

            return Result.Ok(results.Select(i => Mapper.Map<UsageItemDTO>(i)).ToList());
        }

    }
}
