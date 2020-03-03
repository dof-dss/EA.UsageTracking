using System;

namespace EA.UsageTracking.SharedKernel
{
    public abstract class BaseEntity<T>
    {
        protected Guid _tenantId;
        public T Id { get; set; }
    }
}