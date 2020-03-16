using System;
using System.Collections.Generic;
using System.Text;

namespace EA.UsageTracking.SharedKernel.Constants
{
    public static class Constants
    {
        public static class ErrorMessages
        {
            public const string NoTenantExists = "Invalid Tenant";
            public const string NoEventExists = "Event does not exist";
            public const string NoUserExists = "User does not exist";
            public const string NoItemExists = "Item does not exist";
            public const string EmptyGuid = "Application GUID not set";
            public const string NoUserName = "User Name required";
            public const string NoEventName = "Event Name required";
            public const string InvalidPageNumber = "Invalid page number";
            public const string InvalidPageSize = "Invalid page size";
            public const string UpdateException = "Update Exception";
        }

        public static class Tenant
        {
            public const string TenantId = "tenantid";
            public const string TenantIdSwaggerDescription = "Tenant Id, Type: GUID (e.g b0ed668d-7ef2-4a23-a333-94ad278f45d7)";
        }

        public static class ApiRoutes
        {
            public const string Root = "api";
            public class UsageItems
            {
                public const string GetAll = Root + "/applicationUsage";
                public const string GetForUser = Root + "/applicationUsage/user";
            }

            public static class Events
            {
                public const string GetAll = Root + "/applicationEvent";
            }

            public static class Users
            {
                public const string GetAll = Root + "/applicationUser";
            }
        }
    }
}
