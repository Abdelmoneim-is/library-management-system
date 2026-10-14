using LibraryApplication.Common.BorrowingsQueryParams;
using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Specifications
{
    internal class BorrowingFilters
    {
        public static Expression<Func<Borrowing, bool>> Build(BorrowingsQueryParams p) =>
            b => (!p.BookId.HasValue || b.BookId == p.BookId)
              && (string.IsNullOrEmpty(p.UserId) || b.UserId == p.UserId)
              && (p.Status == BorrowingStatusFilter.All
                  || (p.Status == BorrowingStatusFilter.Active && b.ReturnDate == null)
                  || (p.Status == BorrowingStatusFilter.Overdue && b.ReturnDate == null && b.DueDate < DateTime.UtcNow)
                  || (p.Status == BorrowingStatusFilter.Returned && b.ReturnDate != null))
              && (string.IsNullOrWhiteSpace(p.SearchValue)
                  || b.Book.Title.ToLower().Contains(p.SearchValue.ToLower()));
    }
}
