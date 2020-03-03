using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.SharedKernel;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EA.UsageTracking.Infrastructure.Features.Events.Queries
{
    public class GetEventDetailsForApplicationQuery: IRequest<Result<ApplicationEventDTO>>
    {
        public int Id { get; set; }
    }

    public class GetEventDetailsForApplicationQueryHandler : IRequestHandler<GetEventDetailsForApplicationQuery, Result<ApplicationEventDTO>>
    {
        private readonly UsageTrackingContext _dbContext;
        private readonly IMapper _mapper;

        public GetEventDetailsForApplicationQueryHandler(IUsageTrackingContextFactory dbContextFactory, IMapper mapper)
        {
            _dbContext = dbContextFactory.UsageTrackingContext;
            _mapper = mapper;
        }

        public async Task<Result<ApplicationEventDTO>> Handle(GetEventDetailsForApplicationQuery request, CancellationToken cancellationToken)
        {
            var result = await _dbContext.ApplicationEvents
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

            return result.ToMaybe().ToResult(Constants.ErrorMessages.NoEventExists)
                .OnBoth(i => i.IsSuccess
                    ? Result.Ok(_mapper.Map<ApplicationEventDTO>( i.Value))
                    : Result.Fail<ApplicationEventDTO>(i.Error));
        }
    }
}
