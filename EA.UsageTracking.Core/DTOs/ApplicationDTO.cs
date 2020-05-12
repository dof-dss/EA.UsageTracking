using System;
using System.Collections.Generic;
using System.Text;

namespace EA.UsageTracking.Core.DTOs
{
    public class ApplicationDTO
    {
        public int ApplicationId { get; set; }
        public string Name { get; set; }
        public string ClientId { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsRegistered { get; set; }
    }
}
