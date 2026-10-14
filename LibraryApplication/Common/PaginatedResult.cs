using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Common
{
    public sealed class PaginatedResult<TEntity>
    {
        public PaginatedResult(int pageSize, int pageIndex, int count, IReadOnlyList<TEntity> data)
        {
            PageSize = pageSize;
            PageIndex = pageIndex;
            Count = count;
            Data = data;
        }

        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int Count { get; set; }
        public IReadOnlyList<TEntity> Data { get; set; }

        public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(Count / (double)PageSize);
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}
