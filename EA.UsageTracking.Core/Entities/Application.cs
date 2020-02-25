using System;
using System.Collections.Generic;
using System.Text;
using EA.UsageTracking.SharedKernel;

namespace EA.UsageTracking.Core.Entities
{
    public class Application: BaseEntity
    {
        public string Name { get; set; }
        public IEnumerable<ApplicationEvent> ApplicationEvents { get; set; }
        public IEnumerable<ApplicationUser> ApplicationUsers{ get; set; }
        
    }
}
