using LibraryApi.Common;
using LibraryApplication.Common;
using LibraryApplication.Dtos.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Contracts.BooksContracts
{
    public interface IIdentityService
    {
        Task<Result<IdentityUserResult>> FindUserByEmailAsync(string email, CancellationToken ct = default);
        Task<Result<bool>> CheckPasswordAsync(string email, string password, CancellationToken ct = default);
        Task<Result<IReadOnlyList<string>>> GetUserRolesAsync(string email, CancellationToken ct = default);
        Task<Result<IdentityUserResult>> CreateAsync(RegisterDto registerDto, CancellationToken ct = default);
        Task<Result<bool>> AddToRoleAsync(string email, string role, CancellationToken ct = default);
        Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
        Task<Result< bool>> RoleExistAsync(string role, CancellationToken ct = default);
        Task<Dictionary<string, (string UserName, string DisplayName)>> GetUsersInfoByIdsAsync(IEnumerable<string> userIds, CancellationToken ct = default);

    }
}
