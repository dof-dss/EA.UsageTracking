using System;
using EA.UsageTracking.Core.Entities;

namespace EA.UsageTracking.Core.DTOs
{
    public class UsageItemDTO
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public int ApplicationEventId { get; set; }
        public Guid ApplicationUserId { get; set; }
        public string ApplicationUserName { get; set; }
        public string ApplicationEventName { get; set; }
        public string ApplicationName { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsComplete { get; private set; }

    }
}
