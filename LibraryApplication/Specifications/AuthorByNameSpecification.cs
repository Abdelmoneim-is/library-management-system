using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Specifications
{
    internal class AuthorByNameSpecification : BaseSpecification<BookAuthor , int>
    {
        public AuthorByNameSpecification(string fullName, int? excludeId = null)
            : base(a => a.FullName.ToLower() == fullName.ToLower()
                     && (!excludeId.HasValue || a.Id != excludeId.Value))
        {

        }
    }
}
