using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Common
{
    public  enum BookQuerySorting
    {
        None = 0,
        AscendingName = 1,
        DescindingName = 2,
        AscendingPublisherYear = 3,
        DescindingPublisherYear = 4,
    }
}
