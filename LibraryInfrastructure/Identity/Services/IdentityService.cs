using LibraryApi.Common;
using LibraryApplication.Common;
using LibraryApplication.Contracts.BooksContracts;
using LibraryApplication.Dtos.Identity;
using LibraryInfrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryInfrastructure.Identity.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityService(UserManager<AppUser> userManager , RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<Result<bool>> AddToRoleAsync(string email, string role, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Result<bool>.Fail(Errors.NotFound("User Not Found", $"User With Email {email} Is Not Found"));
            }

            var result = await _userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded)
            {
                var error = result.Errors.Select(x => new Errors(x.Code, x.Description, ErrorType.Failure)).ToList();
                return Result<bool>.Fail(error);
            }
            return Result<bool>.Ok(true);
        }

        public async Task<Result<bool>> CheckPasswordAsync(string email, string password, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return Result<bool>.Fail(Errors.NotFound("User Not Found", $"User With Email {email} Is Not Found"));

            if (await _userManager.IsLockedOutAsync(user))
                return Result<bool>.Ok(false);

            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                await _userManager.AccessFailedAsync(user);
                return Result<bool>.Ok(false);
            }

            await _userManager.ResetAccessFailedCountAsync(user);
            return Result<bool>.Ok(true);
        }

        public async Task<Result<IdentityUserResult>> CreateAsync(RegisterDto registerDto, CancellationToken ct = default)
        {
            var user = new AppUser()
            {
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                DisplayName = registerDto.DisplayName,
                UserName = registerDto.UserName,
            };
            var userResult = await _userManager.CreateAsync(user, registerDto.Password);
            if (!userResult.Succeeded)
            {
                var errors = userResult.Errors.Select(x => new Errors (x.Code, x.Description , ErrorType.Failure)).ToList ();
                return Result<IdentityUserResult>.Fail(errors);
            }
            return Result<IdentityUserResult>.Ok(new IdentityUserResult(user.Id, user.DisplayName, user.Email, user.UserName));
        }

        public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
        {
            return await _userManager.FindByEmailAsync(email) is not null;
        }

        public async Task<Result<IdentityUserResult>> FindUserByEmailAsync(string email, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Result<IdentityUserResult>.Fail(Errors.NotFound("User Not Found", $"User With Email {email} Is Not Found"));
            }
            else
            {
                return Result<IdentityUserResult>.Ok(new IdentityUserResult(user.Id, user.DisplayName, user.Email , user.UserName));
            }
        }

        public async Task<Result<IReadOnlyList<string>>> GetUserRolesAsync(string email, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if(user == null)
            {
                return Result<IReadOnlyList<string>>.Fail(Errors.NotFound("User Not Found", $"User With Email {email} Is Not Found"));

            }
            var roles = await _userManager.GetRolesAsync(user);
            return Result<IReadOnlyList<string>>.Ok(roles.ToList());
        }

        public async Task<Dictionary<string, (string UserName, string DisplayName)>> GetUsersInfoByIdsAsync(IEnumerable<string> userIds, CancellationToken ct = default)
        {
            var distinctIds = userIds.Distinct().ToList();

            var users = await _userManager.Users
                .Where(u => distinctIds.Contains(u.Id))
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.DisplayName 
                })
                .ToListAsync(ct);

            return users.ToDictionary(
                u => u.Id,
                u => (UserName: u.UserName ?? string.Empty, DisplayName: u.DisplayName ?? string.Empty)
            );
        }

        public async Task<Result< bool>> RoleExistAsync(string role, CancellationToken ct = default)
        {
            var roleExist = await _roleManager.RoleExistsAsync(role );
            return Result<bool>.Ok(roleExist);
        }
    }
}
