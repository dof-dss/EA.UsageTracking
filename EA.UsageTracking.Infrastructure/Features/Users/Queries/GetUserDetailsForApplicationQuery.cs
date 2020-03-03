using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.Infrastructure.Features.Events.Queries;
using EA.UsageTracking.SharedKernel;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EA.UsageTracking.Infrastructure.Features.Users.Queries
{

    public class GetUserDetailsForApplicationQuery : IRequest<Result<ApplicationUserDTO>>
    {
        public Guid Id { get; set; }
    }

    public class GetUserDetailsForApplicationQueryHandler : IRequestHandler<GetUserDetailsForApplicationQuery, Result<ApplicationUserDTO>>
    {
        private readonly UsageTrackingContext _dbContext;
        private readonly IMapper _mapper;

        public GetUserDetailsForApplicationQueryHandler(IUsageTrackingContextFactory dbContextFactory, IMapper mapper)
        {
            _dbContext = dbContextFactory.UsageTrackingContext;
            _mapper = mapper;
        }

        public async Task<Result<ApplicationUserDTO>> Handle(GetUserDetailsForApplicationQuery request, CancellationToken cancellationToken)
        {
            var result = await _dbContext.ApplicationUsers
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

            return result.ToMaybe().ToResult(Constants.ErrorMessages.NoUserExists)
                .OnBoth(i => i.IsSuccess
                    ? Result.Ok(_mapper.Map<ApplicationUserDTO>(i.Value))
                    : Result.Fail<ApplicationUserDTO>(i.Error));
        }
    }
}
