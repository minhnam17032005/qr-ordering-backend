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
        public string GenerateAccessToken(User user,Guid sessionId)
        {
            // Tạo signing key từ JWT secret
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Key));

            // Cấu hình thuật toán ký HS256
            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            // Tạo các claim cho Access Token
            var claims = new List<Claim>
            {
                new Claim("userId", user.Id.ToString()),
                new Claim("username", user.Username),

                // Liên kết token với UserSession
                new Claim("sid", sessionId.ToString()),

                // Định danh duy nhất của Access Token
                new Claim(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString()),

                // Thời điểm token được tạo
                new Claim(
                    JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow
                        .ToUnixTimeSeconds()
                        .ToString(),
                    ClaimValueTypes.Integer64),

                // Xác định loại tài khoản
                new Claim("user_type", "restaurant_user")
            };

            // Tính thời điểm Access Token hết hạn
            var expires = DateTime.UtcNow.AddMinutes(
                _jwtSettings.AccessTokenExpirationMinutes);

            // Tạo JWT
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials);

            // Serialize JWT thành chuỗi
            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        //Tạo Refresh Token ngẫu nhiên
        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];

            using var rng = RandomNumberGenerator.Create();

            // Sinh dữ liệu ngẫu nhiên an toàn
            rng.GetBytes(randomBytes);

            return Convert.ToBase64String(randomBytes);
        }

        // Tính thời điểm Refresh Token hết hạn
        public DateTime GetRefreshTokenExpiration()
        {
            return DateTime.UtcNow.AddDays(
                _jwtSettings.RefreshTokenExpirationDays);
        }

    }

}
