using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using EA.UsageTracking.SharedKernel;

namespace EA.UsageTracking.Core.Entities
{
    public class ApplicationUser: BaseEntity<Guid>, IBaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public ICollection<UserToApplication> UserToApplications { get; set; } = new List<UserToApplication>();
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
