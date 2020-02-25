using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using EA.UsageTracking.SharedKernel;

namespace EA.UsageTracking.Core.Entities
{
    public class ApplicationUser: BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
