using System;

namespace EA.UsageTracking.SharedKernel.BaseEntity
{
    public abstract class BaseEntity<T>
    {
        public Guid TenantId { get; set; }
        public T Id { get; set; }
    }
}