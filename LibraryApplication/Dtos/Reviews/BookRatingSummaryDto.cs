using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Dtos.Reviews
{
    public class BookRatingSummaryDto
    {
        public int ReviewsCount { get; set; }
        public double AverageRating { get; set; }
    }
}
