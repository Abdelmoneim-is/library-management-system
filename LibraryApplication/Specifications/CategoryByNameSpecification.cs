using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Specifications
{
    internal class CategoryByNameSpecification : BaseSpecification<BookCategory , int>
    {
        public CategoryByNameSpecification(string name, int? excludeId = null)
            : base(c => c.Name.ToLower() == name.ToLower()
                     && (!excludeId.HasValue || c.Id != excludeId.Value))
        {

        }
    }
}
