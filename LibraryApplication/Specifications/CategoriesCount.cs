using LibraryApplication.Common;
using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Specifications
{
    internal class CategoriesCount : BaseSpecification<BookCategory , int>
    {
        public CategoriesCount(CategoryQueryParams queryParams) : base(c => string.IsNullOrWhiteSpace(queryParams.SearchValue)
                || c.Name.ToLower().Contains(queryParams.SearchValue.ToLower())
                || (c.Description != null && c.Description.ToLower().Contains(queryParams.SearchValue.ToLower())))
        {
            
        }
    }
}
