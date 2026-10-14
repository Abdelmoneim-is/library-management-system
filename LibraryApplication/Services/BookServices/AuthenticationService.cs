using LibraryApi.Common;
using LibraryApplication.Contracts.BooksContracts;
using LibraryApplication.Dtos.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Services.BookServices
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;

        public AuthenticationService(IIdentityService identityService , ITokenService tokenService)
        {
            _identityService = identityService;
            _tokenService = tokenService;
        }

        public async Task<Result> AddRoleAsync(RolesDto rolesDto, CancellationToken ct = default)
        {
            var user = await _identityService.FindUserByEmailAsync(rolesDto.Email , ct);
            if (!user.IsSucess)
            {
                return Result.Fail(user.Errors);
            }

            var roles = await _identityService.RoleExistAsync(rolesDto.Role, ct);
            if(!roles.IsSucess)
            {
                return Result.Fail(roles.Errors);
            }
            if (!roles.data)
            {
                return Result.Fail(Errors.NotFound("Role Not Found", $"Role {rolesDto.Role} Is Not Found"));
            }

            var userRole = await _identityService.GetUserRolesAsync(rolesDto.Email, ct);
            if (userRole.IsSucess && userRole.data.Contains(rolesDto.Role))
            {
                return Result.Fail(Errors.Confilict("Roles Already Assigned", $"User Has Already This Role {rolesDto.Role} "));
            }

            var addRoleToUser = await _identityService.AddToRoleAsync(rolesDto.Email, rolesDto.Role, ct);
            if(!addRoleToUser.IsSucess)
            {
                return Result.Fail(addRoleToUser.Errors);
            }
            return Result.Ok();
                
        }

        public async Task<bool> CheckEmailExistsAsync(string email, CancellationToken ct = default)
        {
            return await _identityService.EmailExistsAsync(email, ct);
        }

        public async Task<Result<UserDto>> GetCurrentUserAsync(string email, CancellationToken ct = default)
        {
            var userResult = await _identityService.FindUserByEmailAsync(email, ct);
            if (!userResult.IsSucess)
            {
                return Result<UserDto>.Fail(userResult.Errors);
            }
            var user = userResult.data;


            var roleResult = await _identityService.GetUserRolesAsync(email,ct);
            var roles = roleResult.IsSucess && roleResult.data is not null
               ? roleResult.data
               : new List<string> { "Member" };
            var token = _tokenService.CreateToken(user.Id, user.Email, user.UserName, roles);


            return Result<UserDto>.Ok(new UserDto
            {
                DisplayName = userResult.data.DisplayName,
                Email = userResult.data.Email,
                Token =token ,
                Role = roles.FirstOrDefault() ?? "Member"
            });
        }

        public async Task<Result< UserDto>> LoginAsync(LoginDto loginDto, CancellationToken ct = default)
        {
            var userResult = await _identityService.FindUserByEmailAsync(loginDto.Email , ct);
            if(!userResult.IsSucess)
            {
                return Result<UserDto>.Fail(userResult.Errors);
            }
            var userPassword = await _identityService.CheckPasswordAsync(loginDto.Email , loginDto.Password , ct) ;
            if(!userPassword.IsSucess )
            {
                return Result<UserDto>.Fail(userPassword.Errors);
            }

            if(!userPassword.data)
            {
                return Result<UserDto>.Fail(Errors.Unauthorized("Invalid Email Or Password"));
            }

            var user = userResult.data;

            var roleResult = await _identityService.GetUserRolesAsync(user.Email , ct );
            var roles = roleResult.IsSucess && roleResult.data is not null
                    ? roleResult.data
                    : new List<string> { "Member" };
            var token = _tokenService.CreateToken(user.Id, user.Email, user.UserName, roles);


            var userDto = new UserDto
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = token,
                Role = roles.FirstOrDefault() ?? "Member"   

            };

            return Result<UserDto>.Ok(userDto);
        }

        public async Task<Result<UserDto>> RegisterAsync(RegisterDto registerDto, CancellationToken ct = default)
        {
            var result = await _identityService.CreateAsync(registerDto, ct);
            if (!result.IsSucess)
            {
                return Result<UserDto>.Fail(result.Errors);
            }

            var user = result.data;

            var addRoleResult = await _identityService.AddToRoleAsync(user.Email, "Member", ct);
            if (!addRoleResult.IsSucess)
                return Result<UserDto>.Fail(addRoleResult.Errors);
            var roleResult = await _identityService.GetUserRolesAsync(user.Email , ct);
            var roles = roleResult.IsSucess ? roleResult.data : new List<string> { "Member" };

            var tokens = _tokenService.CreateToken(user.Id, user.Email, user.UserName, roles);

            return Result<UserDto>.Ok(new UserDto
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = tokens,
                Role = roles.FirstOrDefault() ?? "Member"

            });
        }
    }
}
