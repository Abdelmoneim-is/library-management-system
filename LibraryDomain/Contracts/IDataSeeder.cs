using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryDomain.Contracts
{
    public interface IDataSeeder
    {
        Task SeedDataAsync(CancellationToken ct = default);
    }
}
