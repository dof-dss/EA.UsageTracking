using System;
using System.Collections.Generic;
using System.Text;
using EA.UsageTracking.SharedKernel;
using EA.UsageTracking.SharedKernel.BaseEntity;

namespace EA.UsageTracking.Core.Entities
{
    public class Application: BaseEntity<int>, IBaseEntity
    {
        public string Name { get; set; }
        public virtual ICollection<ApplicationEvent> ApplicationEvents { get; set; }
        public ICollection<UserToApplication> UserToApplications { get; set; } = new List<UserToApplication>();
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
