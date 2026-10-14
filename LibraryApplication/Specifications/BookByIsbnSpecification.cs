using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Specifications
{
    internal class BookByIsbnSpecification : BaseSpecification<Books , int>
    {
        public BookByIsbnSpecification(string isbn, int? excludeId = null)
           : base(b => b.ISBN.ToLower() == isbn.ToLower()
                    && (!excludeId.HasValue || b.Id != excludeId.Value))
        { 

        }
    }
}
