using System;
using System.Collections.Generic;
using System.Text;
using EA.UsageTracking.Core.Entities;

namespace EA.UsageTracking.Core.DTOs
{
    public class ApplicationEventDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public static ApplicationEventDTO FromApplicationEvent(ApplicationEvent @event)
        {
            return new ApplicationEventDTO
            {
                Id = @event.Id,
                Name = @event.Name
            };
        }
    }
}
