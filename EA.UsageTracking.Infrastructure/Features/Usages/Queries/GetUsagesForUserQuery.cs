using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.Infrastructure.Features.Common;
using EA.UsageTracking.Infrastructure.Features.Pagination;
using EA.UsageTracking.Infrastructure.Features.Usages.Validation;
using EA.UsageTracking.SharedKernel;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Functional;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EA.UsageTracking.Infrastructure.Features.Usages.Queries
{
    public class GetUsagesForUserQuery : IRequest<Result<PagedResponse<UsageItemDTO>>>
    {
        public Guid Id { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;
        public string ApiRoute { get; set; } = Constants.ApiRoutes.UsageItems.GetForUser;
    }

    public class GetUsagesForUserQueryHandler : BaseHandler<GetUsagesForUserQuery, Result<PagedResponse<UsageItemDTO>>>
    {
        private readonly IUriService _uriService;
        private readonly GetUsagesForUserValidator _validator;

        public GetUsagesForUserQueryHandler(IUsageTrackingContextFactory dbContextFactory, IMapper mapper,
            IUriService uriService)
            : base(dbContextFactory, mapper)
        {
            _uriService = uriService;
            _validator = new GetUsagesForUserValidator();
        }

        protected override Result<PagedResponse<UsageItemDTO>> Handle(GetUsagesForUserQuery message)
        {
            var validationResult = Validate(message);
            if (validationResult.IsFailure)
                return Result.Fail<PagedResponse<UsageItemDTO>>(validationResult.Error);

            var pagination = Mapper.Map<PaginationDetails>(message)
                .WithTotal(DbContext.UsageItems.Count(i => i.ApplicationUser.Id == message.Id));

            var results = DbContext.UsageItems
                .AsNoTracking()
                .Include(a => a.Application)
                .Include(e => e.ApplicationEvent)
                .Include(u => u.ApplicationUser)
                .OrderBy(x => x.Id).ThenBy(y => y.ApplicationUser.Id).ThenBy(z => z.ApplicationEvent.Id)
                .Skip((message.PageNumber - 1) * message.PageSize)
                .Take(message.PageSize)
                .Where(i => i.ApplicationUser.Id == message.Id)
                .Select(i => Mapper.Map<UsageItemDTO>(i))
                .ToList();

            return Result.Ok(PagedResponse<UsageItemDTO>.CreatePaginatedResponse(_uriService, pagination, results));
        }

        protected override Result CustomValidate(GetUsagesForUserQuery request)
        {
            var validationResult = _validator.Validate(request);
            return validationResult.IsValid ? Result.Ok() : Result.Fail(validationResult.ToString(","));
        }
    }
}
