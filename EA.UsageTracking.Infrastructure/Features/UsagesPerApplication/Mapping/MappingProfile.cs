using AutoMapper;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Features.UsagesPerApplication.Queries;

namespace EA.UsageTracking.Infrastructure.Features.UsagesPerApplication.Mapping
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<UsageItem, UsageItemDTO>();
            CreateMap<GetUsagesForApplicationQuery, Pagination.PaginationDetails>();
            CreateMap<GetUsagesForUserQuery, Pagination.PaginationDetails>();
        }
    }
}
