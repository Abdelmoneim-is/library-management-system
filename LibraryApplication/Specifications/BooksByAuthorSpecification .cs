using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Specifications
{
    internal class BooksByAuthorSpecification : BaseSpecification<Books  , int>
    {
        public BooksByAuthorSpecification(int authorId) : base(b => b.AuthorId == authorId) 
        {
        }
    }
}
