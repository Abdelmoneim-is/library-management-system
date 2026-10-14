using LibraryApplication.Common;
using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Specifications
{
    internal class AuthorsCount : BaseSpecification<BookAuthor  , int>
    {
        public AuthorsCount(AuthorQueryParams queryParams) : base(a => string.IsNullOrWhiteSpace(queryParams.SearchValue)
                || a.FullName.ToLower().Contains(queryParams.SearchValue.ToLower())
                || (a.Nationality != null && a.Nationality.ToLower().Contains(queryParams.SearchValue.ToLower())))
        {
            
        }
    }
}
