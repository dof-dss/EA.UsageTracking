using System;
using System.Collections.Generic;
using System.Text;
using EA.UsageTracking.SharedKernel;

namespace EA.UsageTracking.Core.Entities
{
    public class ApplicationEvent: BaseEntity<int>, IBaseEntity
    {
        public Application Application { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
