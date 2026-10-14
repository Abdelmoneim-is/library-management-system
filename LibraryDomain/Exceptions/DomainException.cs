using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryDomain.Exceptions
{
    public abstract class DomainException : Exception
    {
        protected DomainException(string message)
                    : base(message) { }

        protected DomainException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    public class ConcurrencyException : DomainException
    {
        public ConcurrencyException(string message = "A concurrency conflict occurred.")
            : base(message) { }

        public ConcurrencyException(string message, Exception innerException)
            : base(message, innerException) { }

    }
}
