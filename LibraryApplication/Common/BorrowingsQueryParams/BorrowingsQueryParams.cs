using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Common.BorrowingsQueryParams
{
    public class BorrowingsQueryParams
    {
        private const int MaxPageSize = 50;

        private int _pageIndex = 1;
        public int PageIndex
        {
            get => _pageIndex;
            set => _pageIndex = value < 1 ? 1 : value;
        }

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value switch
            {
                < 1 => 1,
                > MaxPageSize => MaxPageSize,
                _ => value
            };
        }

        public BorrowingStatusFilter Status { get; set; }
        public int? BookId { get; set; }
        public string? UserId { get; set; }
        public string? SearchValue { get; set; }   
        public BorrowingQuerySorting Sorting { get; set; }
    }
}
