using LibraryApplication.Contracts.BooksContracts;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LibraryInfrastructure.Identity.Services
{
    public class TokenService : ITokenService
    {
        private readonly JWTSettings _jwtSettings;
        public TokenService(IOptions<JWTSettings> options)
        {
            _jwtSettings = options.Value;
        }
        public string CreateToken(string userId, string email, string userName, IReadOnlyList<string> roles)
        {

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier , userId),
                new Claim(ClaimTypes.Email , email),
                new Claim(ClaimTypes.Name , userName),

            };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));


            var secKey = _jwtSettings.SecretKey;
            if (secKey == null)
                throw new InvalidOperationException("JWT Security Is Missing");
            if(secKey.Length < 32)
                throw new InvalidOperationException("JWT Security Is Missing");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secKey));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                signingCredentials: signingCredentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public class JWTSettings
        {
            public string SecretKey { get; set; } = default!;
            public string Issuer { get; set; } = default!;
            public string Audience { get; set; } = default!;
            public int ExpirationMinutes { get; set; } = default!;
        }
    }
}
