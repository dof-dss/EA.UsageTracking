using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EA.UsageTracking.Infrastructure.Features.Usages.Queries
{

    public class GetUsagesForUserQuery : IRequest<Result<List<UsageItemDTO>>>
    {
        public Guid Id { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;
    }

    public class GetUsagesForUserQueryHandler : RequestHandler<GetUsagesForUserQuery, Result<List<UsageItemDTO>>>
    {
        private readonly UsageTrackingContext _dbContext;
        private IMapper _mapper;

        public GetUsagesForUserQueryHandler(IUsageTrackingContextFactory dbContextFactory, IMapper mapper)
        {
            _dbContext = dbContextFactory.UsageTrackingContext;
            _mapper = mapper;
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
                .Where(i => i.ApplicationUser.Id == message.Id);

            return Result.Ok(results.Select(i => _mapper.Map<UsageItemDTO>(i)).ToList());
        }

    }
}
