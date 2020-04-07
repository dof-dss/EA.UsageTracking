using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Features.Users.Queries;

namespace EA.UsageTracking.Infrastructure.Features.Users.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, ApplicationUserDTO>()
                .ForMember(d => d.Email, o => o.NullSubstitute("N/A"));
            CreateMap<ApplicationUserDTO, ApplicationUser>();
            CreateMap<GetUsersForApplicationQuery, Pagination.PaginationDetails>();
        }
    }
}
