using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.Infrastructure.Features.Events.Queries;
using EA.UsageTracking.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EA.UsageTracking.Infrastructure.Features.Users.Queries
{

    public class GetUsersForApplicationQuery : IRequest<Result<List<ApplicationUserDTO>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;
    }

    public class
        GetUsersForApplicationQueryHandler : RequestHandler<GetUsersForApplicationQuery,
            Result<List<ApplicationUserDTO>>>
    {
        private readonly UsageTrackingContext _dbContext;
        private readonly IMapper _mapper;

        public GetUsersForApplicationQueryHandler(IUsageTrackingContextFactory dbContextFactory, IMapper mapper)
        {
            _dbContext = dbContextFactory.UsageTrackingContext;
            _mapper = mapper;
        }

        protected override Result<List<ApplicationUserDTO>> Handle(GetUsersForApplicationQuery message)
        {
            if (message.PageNumber < 1 || message.PageSize < 1)
                return Result.Fail<List<ApplicationUserDTO>>("Incorrect pagination values");

            var results = _dbContext.ApplicationUsers
                .AsNoTracking()
                .OrderBy(u => u.Id)
                .Skip((message.PageNumber - 1) * message.PageSize)
                .Take(message.PageSize);

            return Result.Ok(results.Select(i => _mapper.Map<ApplicationUserDTO>(i)).ToList());
        }
    }
}
