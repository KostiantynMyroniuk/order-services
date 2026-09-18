using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Models
{
    public record PaginatedList<T>(IReadOnlyList<T> Items, int TotalCount, int PageNumber, int PageSize)
    {
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
