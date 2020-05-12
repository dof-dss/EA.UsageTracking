using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.Infrastructure.Features.Applications.Validation;
using EA.UsageTracking.Infrastructure.Features.Common;
using EA.UsageTracking.Infrastructure.Features.Pagination;
using EA.UsageTracking.Infrastructure.Features.UsagesPerUser.Queries;
using EA.UsageTracking.Infrastructure.Features.UsagesPerUser.Validation;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Functional;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EA.UsageTracking.Infrastructure.Features.Applications.Queries
{
    public class GetAllApplicationsQuery : IRequest<Result<PagedResponse<ApplicationDTO>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;
        public string ApiRoute { get; set; } = Constants.ApiRoutes.UsageItems.GetAllApps;
    }

    public class GetAllApplicationsQueryHandler : RequestHandler<GetAllApplicationsQuery, Result<PagedResponse<ApplicationDTO>>>
    {
        private readonly UsageTrackingContext _usageTrackingContext;
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;
        private readonly GetAllApplicationsValidator _validator;
        private readonly HttpContext _httpContext;

        public GetAllApplicationsQueryHandler(UsageTrackingContext usageTrackingContext, IMapper mapper,
            IUriService uriService, IHttpContextAccessor httpContextAccessor)
        {
            _uriService = uriService;
            _usageTrackingContext = usageTrackingContext;
            _mapper = mapper;
            _validator = new GetAllApplicationsValidator();
            _httpContext = httpContextAccessor.HttpContext;
        }
        protected override Result<PagedResponse<ApplicationDTO>> Handle(GetAllApplicationsQuery request)
        {
            var userIdResult = _httpContext.GetUserId();
            if (userIdResult.IsFailure)
                return Result.Fail<PagedResponse<ApplicationDTO>>(userIdResult.Error);
            var userId = userIdResult.Value;

            var validationResult = Validate(request);
            if (validationResult.IsFailure)
                return Result.Fail<PagedResponse<ApplicationDTO>>(validationResult.Error);

            var pagination = _mapper.Map<PaginationDetails>(request)
                .WithTotal(_usageTrackingContext.Applications.Count());

            var query = _usageTrackingContext.Applications
                .AsNoTracking()
                .IgnoreQueryFilters()
                .Include(a => a.UserToApplications);

            var results = query
                .OrderByDescending(x => x.Id)
                .Skip((pagination.PreviousPageNumber) * pagination.PageSize)
                .Take(pagination.PageSize)
                .AsEnumerable()
                .Select(i =>
                {
                    var applicationDto = _mapper.Map<ApplicationDTO>(i);
                    applicationDto.IsRegistered = i.UserToApplications.Any(u => u.UserId == userId);
                    return applicationDto;
                })
                .ToList();

            return Result.Ok(PagedResponse<ApplicationDTO>.CreatePaginatedResponse(_uriService, pagination, results));
        }

        private Result Validate(GetAllApplicationsQuery request)
        {
            var validationResult = _validator.Validate(request);
            return validationResult.IsValid ? Result.Ok() : Result.Fail(validationResult.ToString(","));
        }
    }
}
