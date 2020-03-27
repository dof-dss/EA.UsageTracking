using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.SharedKernel.Functional;
using MediatR;
using System.Linq;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Infrastructure.Features.Common;
using EA.UsageTracking.Infrastructure.Features.Pagination;
using Microsoft.EntityFrameworkCore;

namespace EA.UsageTracking.Infrastructure.Features.Usages.Queries
{
    public class GetTotalForApplicationQuery: IRequest<Result<int>>
    {
    }

    public class GetTotalForApplicationQueryHandler: AsyncBaseHandler<GetTotalForApplicationQuery>,IRequestHandler<GetTotalForApplicationQuery, Result<int>>
    {

        public GetTotalForApplicationQueryHandler(IUsageTrackingContextFactory dbContextFactory)
        :base(dbContextFactory)
        { }

        public async Task<Result<int>> Handle(GetTotalForApplicationQuery request, CancellationToken cancellationToken)
        {
            var validationResult = Validate(request);
            if (validationResult.IsFailure)
                return Result.Fail<int>(validationResult.Error);

            var total = await DbContext.UsageItems.CountAsync(cancellationToken);
            return Result.Ok(total);
        }

        protected override Result CustomValidate(GetTotalForApplicationQuery request)
        {
            return Result.Ok();
        }
    }
}
