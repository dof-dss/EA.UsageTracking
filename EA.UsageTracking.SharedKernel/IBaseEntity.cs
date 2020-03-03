using System;

namespace EA.UsageTracking.SharedKernel
{
    public interface IBaseEntity
    {
        DateTime DateCreated { get; set; }
        DateTime DateModified { get; set; }
    }
}