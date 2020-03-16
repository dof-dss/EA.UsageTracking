using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.Infrastructure.Features.Common;
using EA.UsageTracking.SharedKernel;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Extensions;
using EA.UsageTracking.SharedKernel.Functional;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EA.UsageTracking.Infrastructure.Features.Events.Queries
{
    public class GetEventDetailsForApplicationQuery: IRequest<Result<ApplicationEventDTO>>
    {
        public int Id { get; set; }
    }

    public class GetEventDetailsForApplicationQueryHandler :
        AsyncBaseHandler<GetEventDetailsForApplicationQuery>, IRequestHandler<GetEventDetailsForApplicationQuery, Result<ApplicationEventDTO>>
    {

        public GetEventDetailsForApplicationQueryHandler(IUsageTrackingContextFactory dbContextFactory, IMapper mapper)
        : base(dbContextFactory, mapper)
        {
        }

        public async Task<Result<ApplicationEventDTO>> Handle(GetEventDetailsForApplicationQuery request, CancellationToken cancellationToken)
        {
            var validationResults = Validate(request);
            if (validationResults.IsFailure)
                return Result.Fail<ApplicationEventDTO>(validationResults.Error);

            var applicationEvent = await DbContext.ApplicationEvents
                .AsNoTracking()
                .SingleAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

            return Result.Ok(Mapper.Map<ApplicationEventDTO>(applicationEvent));
        }

        protected override Result CustomValidate(GetEventDetailsForApplicationQuery request) =>
            DbContext.ApplicationEvents
                .Any(x => x.Id == request.Id)
                ? Result.Ok()
                : Result.Fail(Constants.ErrorMessages.NoEventExists);
    }
}
