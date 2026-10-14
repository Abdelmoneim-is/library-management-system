using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Common
{
    public static class BorrowingConstants
    {
        public const int DefaultBorrowingDurationDays = 14;
        public const int MaxActiveBorrowingsPerUser = 3;
        public const int MaxExtensionCount = 1;              
        public const int ExtensionDurationDays = 7;       
        public const decimal DailyFineAmount = 10.0m;    
    }
}
