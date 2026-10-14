using LibraryApplication.Common.BorrowingsQueryParams;
using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Specifications
{
    internal class BorrowingsAdminSpecification : BaseSpecification<Borrowing, int>
    {
        public BorrowingsAdminSpecification(BorrowingsQueryParams queryParams)
            : base(BorrowingFilters.Build(queryParams))
        {
            AddInclude(b => b.Book);

            switch (queryParams.Sorting)
            {
                case BorrowingQuerySorting.Oldest:
                    AddOrder(b => b.BorrowDate);
                    break;
                case BorrowingQuerySorting.DueDateAscending:
                    AddOrder(b => b.DueDate);
                    break;
                case BorrowingQuerySorting.DueDateDescending:
                    AddOrderByDescinding(b => b.DueDate);
                    break;
                default:
                    AddOrderByDescinding(b => b.BorrowDate);
                    break;
            }

            ApplyPagination(queryParams.PageSize, queryParams.PageIndex);
        }
    }
}
