using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EA.UsageTracking.Infrastructure.Features.Events.Queries
{
    public class GetEventsForApplicationQuery: IRequest<Result<List<ApplicationEventDTO>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;
    }

    public class GetEventsForApplicationQueryHandler : RequestHandler<GetEventsForApplicationQuery, Result<List<ApplicationEventDTO>>>
    {
        private readonly UsageTrackingContext _dbContext;
        private readonly IMapper _mapper;

        public GetEventsForApplicationQueryHandler(IUsageTrackingContextFactory dbContextFactory, IMapper mapper)
        {
            _dbContext = dbContextFactory.UsageTrackingContext;
            _mapper = mapper;
        }

        protected override Result<List<ApplicationEventDTO>> Handle(GetEventsForApplicationQuery message)
        {
            if (message.PageNumber < 1 || message.PageSize < 1)
                return Result.Fail<List<ApplicationEventDTO>>("Incorrect pagination values");

            var results = _dbContext.ApplicationEvents
                .AsNoTracking()
                .OrderBy(u => u.Id)
                .Skip((message.PageNumber - 1) * message.PageSize)
                .Take(message.PageSize);

            return Result.Ok(results.Select(i => _mapper.Map<ApplicationEventDTO>(i)).ToList());
        }
    }
}
