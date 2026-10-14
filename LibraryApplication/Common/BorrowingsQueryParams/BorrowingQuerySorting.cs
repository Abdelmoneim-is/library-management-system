using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Common.BorrowingsQueryParams
{
    public enum BorrowingQuerySorting
    {
        Newest = 0,           
        Oldest = 1,
        DueDateAscending = 2,   
        DueDateDescending = 3
    }
}
