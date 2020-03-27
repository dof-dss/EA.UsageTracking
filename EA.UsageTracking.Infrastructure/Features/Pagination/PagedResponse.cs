using System.Collections.Generic;
using System.Linq;

namespace EA.UsageTracking.Infrastructure.Features.Pagination
{
    public class PagedResponse<T>
    {
        private PagedResponse() { }

        public IEnumerable<T> Data { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public string Total { get; set; }
        public string NextPage { get; set; }
        public string PreviousPage { get; set; }

        public static PagedResponse<T> CreatePaginatedResponse(IUriService uriService, PaginationDetails paginationDetails, List<T> response)
        {
            var nextPage = paginationDetails.PageNumber >= 1
                ? uriService.CreateNextPageUri(paginationDetails).ToString()
                : null;

            var previousPage = paginationDetails.PreviousPageNumber >= 1
                ? uriService.CreatePreviousPageUri(paginationDetails).ToString()
                : null;

            return new PagedResponse<T>
            {
                Data = response,
                PageNumber = paginationDetails.PageNumber >= 1 ? paginationDetails.PageNumber : (int?)null,
                PageSize = paginationDetails.PageSize >= 1 ? paginationDetails.PageSize : (int?)null,
                NextPage = response.Any() ? nextPage : null,
                PreviousPage = previousPage,
                Total = paginationDetails.Total == 0 ? "Not available" : paginationDetails.Total.ToString()
            };
        }
    }
}
