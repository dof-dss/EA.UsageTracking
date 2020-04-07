using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.Infrastructure.Features.Common;
using EA.UsageTracking.Infrastructure.Features.Events.Queries;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Functional;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EA.UsageTracking.Infrastructure.Features.Applications.Queries
{
    public class GetApplicationQuery : IRequest<Result<ApplicationDTO>>
    {
    }

    public class GetApplicationQueryHandler : AsyncBaseHandler<GetApplicationQuery>,
        IRequestHandler<GetApplicationQuery, Result<ApplicationDTO>>
    {
        public GetApplicationQueryHandler(IUsageTrackingContextFactory dbContextFactory, IMapper mapper)
            : base(dbContextFactory, mapper)
        {
        }

        public async Task<Result<ApplicationDTO>> Handle(GetApplicationQuery request, CancellationToken cancellationToken)
        {
            var validationResults = Validate(request);
            if (validationResults.IsFailure)
                return Result.Fail<ApplicationDTO>(validationResults.Error);

            var application = await DbContext.Applications
                .AsNoTracking()
                .SingleAsync(cancellationToken: cancellationToken);

            return Result.Ok(Mapper.Map<ApplicationDTO>(application));
        }

        protected override Result CustomValidate(GetApplicationQuery request) =>
            Result.Ok();

    }
}
