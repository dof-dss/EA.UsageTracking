using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using EA.UsageTracking.SharedKernel;
using EA.UsageTracking.SharedKernel.BaseEntity;

namespace EA.UsageTracking.Core.Entities
{
    public class ApplicationUser: IBaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public ICollection<UserToApplication> UserToApplications { get; set; } = new List<UserToApplication>();
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
