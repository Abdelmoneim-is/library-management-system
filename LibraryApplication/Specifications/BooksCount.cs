using LibraryApplication.Common;
using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Specifications
{
    internal class BooksCount : BaseSpecification<Books , int>
    {
        public BooksCount(BooksQueryParams queryParams) : base(b => (!queryParams.AuthorId.HasValue || b.AuthorId == queryParams.AuthorId)
                 && (!queryParams.CategoryId.HasValue || b.CategoryId == queryParams.CategoryId)
                 && (string.IsNullOrWhiteSpace(queryParams.SearchValue)
                     || b.Title.ToLower().Contains(queryParams.SearchValue.ToLower())
                     || b.ISBN.ToLower().Contains(queryParams.SearchValue.ToLower())
                     || b.Publisher.ToLower().Contains(queryParams.SearchValue.ToLower())))
        {
            
        }
    }
}
