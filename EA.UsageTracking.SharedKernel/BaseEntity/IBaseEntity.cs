using System;

namespace EA.UsageTracking.SharedKernel.BaseEntity
{
    public interface IBaseEntity
    {
        DateTime DateCreated { get; set; }
        DateTime DateModified { get; set; }
    }
}