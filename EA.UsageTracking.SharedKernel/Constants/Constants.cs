using System;
using System.Collections.Generic;
using System.Text;

namespace EA.UsageTracking.SharedKernel.Constants
{
    public static class Constants
    {
        public class ErrorMessages
        {
            public const string NoTenantExists = "Invalid Tenant";
            public const string NoEventExists = "Event does not exist";
            public const string NoUserExists = "User does not exist";
            public const string NoItemExists = "Item does not exist";
        }

        public class Tenant
        {
            public const string TenantId = "tenantid";
            public const string TenantIdSwaggerDescription = "Tenant Id, Type: GUID (e.g b0ed668d-7ef2-4a23-a333-94ad278f45d7)";
        }
    }
}
