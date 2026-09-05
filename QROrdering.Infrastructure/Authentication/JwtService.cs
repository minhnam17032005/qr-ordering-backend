using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using QROrdering.Application.Authentication.Interfaces;
using QROrdering.Domain.Entities.Identity;
using QROrdering.Infrastructure.Configurations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace QROrdering.Infrastructure.Authentication
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtService(IOptions<JwtSettings> jwtOptions)
        {
            _jwtSettings = jwtOptions.Value;
        }

        //tạo Access Token
        public string GenerateAccessToken(
            User user,
            Guid sessionId)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Key));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim("userId", user.Id.ToString()),
                new Claim("username", user.Username),
                new Claim("sid", sessionId.ToString()),

                new Claim(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString()),

                new Claim(
                    JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow
                        .ToUnixTimeSeconds()
                        .ToString(),
                    ClaimValueTypes.Integer64),

                new Claim("user_type", "restaurant_user")
            };

            var expires = DateTime.UtcNow.AddMinutes(
                _jwtSettings.AccessTokenExpirationMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        //tạo Refresh Token
        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];

            using var rng = RandomNumberGenerator.Create();

            rng.GetBytes(randomBytes);

            return Convert.ToBase64String(randomBytes);
        }

        public DateTime GetRefreshTokenExpiration()
        {
            return DateTime.UtcNow.AddDays(
                _jwtSettings.RefreshTokenExpirationDays);
        }

    }

}
