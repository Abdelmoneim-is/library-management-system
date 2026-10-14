using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Specifications
{
    internal class BooksByCategorySpecification  : BaseSpecification<Books , int>
    {
        public BooksByCategorySpecification(int categoryId) : base(b => b.CategoryId == categoryId) 
        {
        }
    }
}
