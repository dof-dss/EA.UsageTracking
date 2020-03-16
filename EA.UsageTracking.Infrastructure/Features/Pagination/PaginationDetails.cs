using System;
using System.Collections.Generic;
using System.Text;

namespace EA.UsageTracking.Infrastructure.Features.Pagination
{
    public class PaginationDetails
    {
        public string ApiRoute { get; set; }
        public int PageNumber { get; set; }
        public int PreviousPageNumber => PageNumber - 1;
        public int NextPageNumber => PageNumber + 1;
        public int PageSize { get; set; }
        public int Total { get; private set; }

        public PaginationDetails WithTotal(int total)
        {
            Total = total;
            return this;
        }
    }
}
