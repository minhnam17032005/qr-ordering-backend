using QROrdering.Application.Authentication.Interfaces;

namespace QROrdering.Infrastructure.Authentication
{
    public class PasswordService : IPasswordService
    {
        // Hash password bằng BCrypt
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Kiểm tra password với hash đã lưu
        public bool Verify(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}