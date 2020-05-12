using System;
using System.Linq;
using AutoMapper;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.Infrastructure.Features.Common;
using EA.UsageTracking.Infrastructure.Features.Pagination;
using EA.UsageTracking.Infrastructure.Features.UsagesPerApplication.Validation;
using EA.UsageTracking.Infrastructure.Features.UsagesPerUser.Validation;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Extensions;
using EA.UsageTracking.SharedKernel.Functional;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EA.UsageTracking.Infrastructure.Features.UsagesPerUser.Queries
{
    public class GetUsagesQuery : IRequest<Result<PagedResponse<UsageItemDTO>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;
        public string ApiRoute { get; set; } = Constants.ApiRoutes.UsageItems.GetPerUser;
    }

    public class GetUsagesQueryHandler : RequestHandler<GetUsagesQuery, Result<PagedResponse<UsageItemDTO>>>
    {
        private readonly IUriService _uriService;
        private readonly GetUsagesValidator _validator;
        private readonly UsageTrackingContext _usageTrackingContext;
        private readonly IMapper _mapper;
        private readonly HttpContext _httpContext;

        public GetUsagesQueryHandler(UsageTrackingContext usageTrackingContext, IMapper mapper,
            IUriService uriService, IHttpContextAccessor httpContextAccessor)
        {
            _uriService = uriService;
            _usageTrackingContext = usageTrackingContext;
            _mapper = mapper;
            _validator = new GetUsagesValidator();
            _httpContext = httpContextAccessor.HttpContext;
        }

        protected override Result<PagedResponse<UsageItemDTO>> Handle(GetUsagesQuery message)
        {
            var userIdResult = _httpContext.GetUserId();
            if (userIdResult.IsFailure)
                return Result.Fail<PagedResponse<UsageItemDTO>>(userIdResult.Error);
            var userId = userIdResult.Value;

            var validationResult = Validate(message);
            if (validationResult.IsFailure)
                return Result.Fail<PagedResponse<UsageItemDTO>>(validationResult.Error);

            var pagination = _mapper.Map<PaginationDetails>(message)
                .WithTotal(_usageTrackingContext.UsageItems.IgnoreQueryFilters().Count(i => i.ApplicationUserId == userId));

            var query = _usageTrackingContext.UsageItems
                .AsNoTracking()
                .IgnoreQueryFilters()
                .Include(a => a.Application)
                .Include(e => e.ApplicationEvent)
                .Include(u => u.ApplicationUser)
                .Where(i => i.ApplicationUserId == userId);

            var results = query
                .OrderByDescending(x => x.ApplicationId).ThenBy(x => x.Id)
                .Skip((pagination.PreviousPageNumber) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Select(i => _mapper.Map<UsageItemDTO>(i))
                .ToList();

            return Result.Ok(PagedResponse<UsageItemDTO>.CreatePaginatedResponse(_uriService, pagination, results));
        }

        private Result Validate(GetUsagesQuery request)
        {
            var validationResult = _validator.Validate(request);
            return validationResult.IsValid ? Result.Ok() : Result.Fail(validationResult.ToString(","));
        }
    }
}
