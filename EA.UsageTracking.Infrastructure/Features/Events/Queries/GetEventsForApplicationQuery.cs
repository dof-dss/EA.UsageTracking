using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.Infrastructure.Features.Common;
using EA.UsageTracking.Infrastructure.Features.Events.Validation;
using EA.UsageTracking.Infrastructure.Features.Pagination;
using EA.UsageTracking.SharedKernel;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Functional;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EA.UsageTracking.Infrastructure.Features.Events.Queries
{
    public class GetEventsForApplicationQuery: IRequest<Result<PagedResponse<ApplicationEventDTO>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;
        public string ApiRoute { get; set; } = Constants.ApiRoutes.Events.GetAll;
    }

    public class GetEventsForApplicationQueryHandler : BaseHandler<GetEventsForApplicationQuery, Result<PagedResponse<ApplicationEventDTO>>>
    {
        private readonly IUriService _uriService;
        private readonly GetEventsForApplicationValidator _validator;

        public GetEventsForApplicationQueryHandler(IUsageTrackingContextFactory dbContextFactory, IMapper mapper, IUriService uriService)
        : base(dbContextFactory, mapper)
        {
            _uriService = uriService;
            _validator = new GetEventsForApplicationValidator();
        }

        protected override Result<PagedResponse<ApplicationEventDTO>> Handle(GetEventsForApplicationQuery message)
        {
            var validationResults = Validate(message);
            if (validationResults.IsFailure)
                return Result.Fail<PagedResponse<ApplicationEventDTO>>(validationResults.Error);

            var pagination = Mapper.Map<PaginationDetails>(message).WithTotal(DbContext.ApplicationEvents.Count());

            var results = DbContext.ApplicationEvents
                .AsNoTracking()
                .OrderBy(u => u.Id)
                .Skip((message.PageNumber - 1) * message.PageSize)
                .Take(message.PageSize)
                .Select(e => Mapper.Map<ApplicationEventDTO>(e))
                .ToList();

            return Result.Ok(PagedResponse<ApplicationEventDTO>.CreatePaginatedResponse(_uriService, pagination, results));
        }

        protected override Result CustomValidate(GetEventsForApplicationQuery request)
        {
            var validationResults = _validator.Validate(request);
            return (!validationResults.IsValid? Result.Fail(validationResults.ToString(",")) : Result.Ok());
        }
    }
}
