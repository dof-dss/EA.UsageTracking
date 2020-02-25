using System;
using System.Collections.Generic;
using System.Text;
using EA.UsageTracking.SharedKernel;

namespace EA.UsageTracking.Core.Entities
{
    public class ApplicationEvent: BaseEntity
    {
        public string Name { get; set; }
    }
}
