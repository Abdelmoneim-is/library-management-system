using LibraryApi.Common;
using LibraryApplication.Dtos.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Contracts.BooksContracts
{
    public interface IAuthenticationService
    {
        Task<Result< UserDto>> LoginAsync(LoginDto loginDto, CancellationToken ct = default);
        Task<Result<UserDto>> RegisterAsync(RegisterDto registerDto, CancellationToken ct = default);

        Task<Result<UserDto>> GetCurrentUserAsync(string email, CancellationToken ct = default); 
        Task<bool> CheckEmailExistsAsync(string email, CancellationToken ct = default);
        Task<Result> AddRoleAsync(RolesDto rolesDto, CancellationToken ct = default);
    }
}

