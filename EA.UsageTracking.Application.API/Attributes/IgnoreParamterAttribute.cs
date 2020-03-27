using System;

namespace EA.UsageTracking.Application.API.Attributes
{
    public class IgnoreParameterAttribute : Attribute
    {
        public string ParameterToIgnore { get; set; }
    }
}