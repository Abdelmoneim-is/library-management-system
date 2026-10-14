using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Contracts.BooksContracts
{
    public interface ITokenService
    {
        string CreateToken(string userId, string email, string userName, IReadOnlyList<string> roles);
    }
}
