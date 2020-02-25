using System;
using EA.UsageTracking.Core.Entities;

namespace EA.UsageTracking.Core.DTOs
{
    public class UsageItemDTO
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public int ApplicationEventId { get; set; }
        public int ApplicationUserId { get; set; }
        public string ApplicationUserName { get; set; }
        public string ApplicationEventName { get; set; }
        public string ApplicationName { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsComplete { get; private set; }

        public static UsageItemDTO FromUsageItem(UsageItem item)
        {
            return new UsageItemDTO()
            {
                Id = item.Id,
                ApplicationId = item.Application.Id,
                ApplicationEventId = item.ApplicationEvent.Id,
                ApplicationUserId = item.ApplicationUser.Id,
                ApplicationName = item.Application.Name,
                ApplicationEventName = item.ApplicationEvent.Name,
                ApplicationUserName = item.ApplicationUser.Name,
                DateCreated = item.DateCreated,
                IsComplete = item.IsComplete
            };
        }
    }
}
