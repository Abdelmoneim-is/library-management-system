using LibraryApplication.Common;
using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Specifications
{
    internal class CategoriesSpecifications : BaseSpecification<BookCategory , int>
    {
        public CategoriesSpecifications(CategoryQueryParams queryParams) : base(c => string.IsNullOrWhiteSpace(queryParams.SearchValue)
                || c.Name.ToLower().Contains(queryParams.SearchValue.ToLower())
                || (c.Description != null && c.Description.ToLower().Contains(queryParams.SearchValue.ToLower())))
        {
            switch (queryParams.Sorting)
            {
                case CategoryQuerySorting.DescindingName:
                    AddOrderByDescinding(c => c.Name);
                    break;
                case CategoryQuerySorting.AscendingName:
                    AddOrder(c => c.Name);
                    break;
                default:
                    AddOrder(c => c.Id);
                    break;
            }

            ApplyPagination(queryParams.PageSize, queryParams.PageIndex);

        }
    }
}
