using System;

namespace EA.UsageTracking.Infrastructure.Features.Pagination
{
    public interface IUriService
    {
        Uri CreateNextPageUri(PaginationDetails paginationDetails);
        Uri CreatePreviousPageUri(PaginationDetails paginationDetails);
    }
}
