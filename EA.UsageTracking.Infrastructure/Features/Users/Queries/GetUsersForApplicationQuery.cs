using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.Infrastructure.Features.Common;
using EA.UsageTracking.Infrastructure.Features.Events.Queries;
using EA.UsageTracking.Infrastructure.Features.Pagination;
using EA.UsageTracking.Infrastructure.Features.Users.Validation;
using EA.UsageTracking.SharedKernel;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Extensions;
using EA.UsageTracking.SharedKernel.Functional;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EA.UsageTracking.Infrastructure.Features.Users.Queries
{

    public class GetUsersForApplicationQuery : IRequest<Result<PagedResponse<ApplicationUserDTO>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;
        public string ApiRoute { get; set; } = Constants.ApiRoutes.Users.GetAll;
    }
    public class GetUsersForApplicationQueryHandler : BaseHandler<GetUsersForApplicationQuery, Result<PagedResponse<ApplicationUserDTO>>>
    {
        private readonly IUriService _uriService;
        private readonly GetUsersForApplicationValidator _validator;

        public GetUsersForApplicationQueryHandler(IUsageTrackingContextFactory dbContextFactory, IMapper mapper, IUriService uriService)
        : base(dbContextFactory, mapper)
        {
            _uriService = uriService;
            _validator = new GetUsersForApplicationValidator();
        }

        protected override Result<PagedResponse<ApplicationUserDTO>> Handle(GetUsersForApplicationQuery message)
        {
            var validationResult = Validate(message);
            if (validationResult.IsFailure)
                return Result.Fail<PagedResponse<ApplicationUserDTO>>(validationResult.Error);

            var pagination = Mapper.Map<PaginationDetails>(message)
                .WithTotal(DbContext.ApplicationUsers.Count(ua => ua.UserToApplications.Any(a => a.Application.TenantId == DbContext.TenantId)));

            var results = DbContext.ApplicationUsers
                .AsNoTracking()
                .OrderBy(u => u.Id)
                .Skip(pagination.PreviousPageNumber * message.PageSize)
                .Take(pagination.PageSize)
                .Where(ua => ua.UserToApplications.Any(a => a.Application.TenantId == DbContext.TenantId))
                .Select(u => Mapper.Map<ApplicationUserDTO>(u))
                .ToList();

            return Result.Ok(PagedResponse<ApplicationUserDTO>.CreatePaginatedResponse(_uriService, pagination, results));
        }

        protected override Result CustomValidate(GetUsersForApplicationQuery request)
        {
            var validationResult = _validator.Validate(request);
            return validationResult.IsValid ? Result.Ok() : Result.Fail(validationResult.ToString(","));
        }
    }
}
