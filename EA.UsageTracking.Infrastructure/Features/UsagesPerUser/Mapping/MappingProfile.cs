using AutoMapper;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Features.UsagesPerApplication.Queries;
using EA.UsageTracking.Infrastructure.Features.UsagesPerUser.Queries;

namespace EA.UsageTracking.Infrastructure.Features.UsagesPerUser.Mapping
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<UsageItem, UsageItemDTO>();
            CreateMap<GetUsagesQuery, Pagination.PaginationDetails>();
            CreateMap<GetUsagesAppQuery, Pagination.PaginationDetails>();
            CreateMap<GetAppsQuery, Pagination.PaginationDetails>();
        }
    }
}
