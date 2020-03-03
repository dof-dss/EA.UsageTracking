using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Core.Entities;

namespace EA.UsageTracking.Infrastructure.Features.Events.Mapping
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationEvent, ApplicationEventDTO>();
            CreateMap<ApplicationEventDTO, ApplicationEvent>();
        }
    }
}
