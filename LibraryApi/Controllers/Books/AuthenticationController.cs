using LibraryApi.Common;
using LibraryApplication.Contracts.BooksContracts;
using LibraryApplication.Dtos.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace LibraryApi.Controllers.Books
{

    public class AuthenticationController : APIBaseController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [EnableRateLimiting("auth")]
        [HttpPost("Login")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserDto>> Login (LoginDto loginDto , CancellationToken ct)
        {
            return ToActionResult(await _authenticationService.LoginAsync(loginDto , ct));
        }


        [EnableRateLimiting("auth")]
        [HttpPost("Register")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDto>> Register (RegisterDto registerDto , CancellationToken ct)
        {
            return ToActionResult(await _authenticationService.RegisterAsync(registerDto , ct));
        }


        [HttpGet("EmailExists")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> CheckEmailExists([FromQuery] string email, CancellationToken ct)
        {
            return Ok(await _authenticationService.CheckEmailExistsAsync(email, ct));
        }

        [Authorize]
        [HttpGet("CurrentUser")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserDto>> GetCurrentUser(CancellationToken ct)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            return ToActionResult(await _authenticationService.GetCurrentUserAsync(email, ct));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddRole")]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> AddRoleToUser (RolesDto rolesDto , CancellationToken ct)
        {
            var result = await _authenticationService.AddRoleAsync(rolesDto, ct);
            return ToActionResult(result, $"Role '{rolesDto.Role}' assigned to {rolesDto.Email} successfully.");
        }
    }
}
