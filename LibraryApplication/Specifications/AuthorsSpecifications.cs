using LibraryApplication.Common;
using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Specifications
{
    internal class AuthorsSpecifications : BaseSpecification<BookAuthor , int>
    {
        public AuthorsSpecifications(AuthorQueryParams queryParams) : base(a => string.IsNullOrWhiteSpace(queryParams.SearchValue)
                || a.FullName.ToLower().Contains(queryParams.SearchValue.ToLower())
                || (a.Nationality != null && a.Nationality.ToLower().Contains(queryParams.SearchValue.ToLower())))
        {
            switch (queryParams.Sorting)
            {
                case AuthorQuerySoring.DescindingName:
                    AddOrderByDescinding(a => a.FullName);
                    break;
                case AuthorQuerySoring.AscendingName:
                    AddOrder(a => a.FullName);
                    break;
                default:
                    AddOrder(a => a.Id);
                    break;
            }
            ApplyPagination(queryParams.PageSize, queryParams.PageIndex);

        }
    }
}
