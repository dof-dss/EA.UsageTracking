using System;
using EA.UsageTracking.SharedKernel;

namespace EA.UsageTracking.Core.Entities
{
    public class UserToApplication
    {
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int ApplicationId { get; set; }
        public Application Application { get; set; }
    }
}