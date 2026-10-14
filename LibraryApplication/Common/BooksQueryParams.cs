using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Common
{
    public class BooksQueryParams
    {
        public int? AuthorId { get; set; }
        public int? CategoryId { get; set; }
        public string? SearchValue { get; set; }
        public BookQuerySorting Sorting { get; set; }

        public int PageIndex { get; set; } = 1;


        private const int  DefaultPageSize = 2;
        private const int MaxPageSize = 5;
        private int  pageSize = DefaultPageSize;

        public int PageSize
        {
            get => pageSize;
            set => pageSize = value > MaxPageSize ? MaxPageSize : (value < 1 ? DefaultPageSize : value);
        }
    }
}
