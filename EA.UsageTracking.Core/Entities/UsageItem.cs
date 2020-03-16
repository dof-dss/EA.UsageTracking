
using System;
using EA.UsageTracking.SharedKernel;
using EA.UsageTracking.SharedKernel.BaseEntity;

namespace EA.UsageTracking.Core.Entities
{
    public class UsageItem : BaseEntity<int>, IBaseEntity
    {
        public int ApplicationId { get; set; }
        public Application Application { get; set; }
        public int ApplicationEventId { get; set; }
        public ApplicationEvent ApplicationEvent { get; set; }
        public Guid ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public bool IsComplete { get; set; }

        public void MarkComplete()
        {
            IsComplete = true;
        }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
