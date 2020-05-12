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
using EA.UsageTracking.Infrastructure.Features.Pagination;
using EA.UsageTracking.Infrastructure.Features.UsagesPerUser.Validation;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Extensions;
using EA.UsageTracking.SharedKernel.Functional;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EA.UsageTracking.Infrastructure.Features.UsagesPerUser.Queries
{
    public class GetAppsQuery: IRequest<Result<PagedResponse<ApplicationDTO>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;
        public string ApiRoute { get; set; } = Constants.ApiRoutes.UsageItems.GetAppsPerUser;
    }

    public class GetAppsQueryHandler : RequestHandler<GetAppsQuery, Result<PagedResponse<ApplicationDTO>>>
    {
        private readonly UsageTrackingContext _usageTrackingContext;
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;
        private readonly HttpContext _httpContext;
        private readonly GetAppsValidator _validator;

        public GetAppsQueryHandler(UsageTrackingContext usageTrackingContext, IMapper mapper,
            IUriService uriService, IHttpContextAccessor httpContextAccessor)
        {
            _uriService = uriService;
            _usageTrackingContext = usageTrackingContext;
            _mapper = mapper;
            _validator = new GetAppsValidator();
            _httpContext = httpContextAccessor.HttpContext;
        }
        protected override Result<PagedResponse<ApplicationDTO>> Handle(GetAppsQuery request)
        {
            var userIdResult = _httpContext.GetUserId();
            if (userIdResult.IsFailure)
                return Result.Fail<PagedResponse<ApplicationDTO>>(userIdResult.Error);
            var userId = userIdResult.Value;

            var validationResult = Validate(request);
            if (validationResult.IsFailure)
                return Result.Fail<PagedResponse<ApplicationDTO>>(validationResult.Error);

            var pagination = _mapper.Map<PaginationDetails>(request)
                .WithTotal(_usageTrackingContext.Applications
                    .Include(x => x.UserToApplications)
                    .Count(i => i.UserToApplications.Any(x => x.UserId == userId)));

            var query = _usageTrackingContext.Applications
                .AsNoTracking()
                .IgnoreQueryFilters()
                .Include(a => a.UserToApplications)
                .Include(e => e.ApplicationEvents)
                .Where(i => i.UserToApplications.Any(x => x.UserId == userId));

            var results = query
                .OrderByDescending(x => x.Id)
                .Skip((pagination.PreviousPageNumber) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Select(i => _mapper.Map<ApplicationDTO>(i))
                .ToList();

            return Result.Ok(PagedResponse<ApplicationDTO>.CreatePaginatedResponse(_uriService, pagination, results));
        }

        private Result Validate(GetAppsQuery request)
        {
            var validationResult = _validator.Validate(request);
            return validationResult.IsValid ? Result.Ok() : Result.Fail(validationResult.ToString(","));
        }
    }
}
