
using EA.UsageTracking.SharedKernel;

namespace EA.UsageTracking.Core.Entities
{
    public class UsageItem : BaseEntity
    {
        public Application Application { get; set; } 
        public ApplicationEvent ApplicationEvent { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public bool IsComplete { get; set; }

        public void MarkComplete()
        {
            IsComplete = true;
        }
    }
}
