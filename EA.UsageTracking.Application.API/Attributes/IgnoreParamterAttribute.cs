using System;

namespace EA.UsageTracking.Application.API.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class IgnoreParameterAttribute : Attribute
    {
        public string ParameterToIgnore { get; set; }
    }
}