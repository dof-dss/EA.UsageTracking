
using System;
using EA.UsageTracking.SharedKernel;

namespace EA.UsageTracking.Core.Entities
{
    public class UsageItem : BaseEntity<int>, IBaseEntity
    {
        public Application Application { get; set; } 
        public ApplicationEvent ApplicationEvent { get; set; }
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
