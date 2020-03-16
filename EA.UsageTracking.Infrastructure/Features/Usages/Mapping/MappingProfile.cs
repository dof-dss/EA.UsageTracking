using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Features.Usages.Queries;

namespace EA.UsageTracking.Infrastructure.Features.Usages.Mapping
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
