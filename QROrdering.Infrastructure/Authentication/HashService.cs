using System.Security.Cryptography;
using System.Text;
using QROrdering.Application.Authentication.Interfaces;

namespace QROrdering.Infrastructure.Authentication
{
    public class HashService : IHashService
    {
        public string Hash(string value)
        {
            using var sha256 = SHA256.Create();

            var bytes = Encoding.UTF8.GetBytes(value);
            var hash = sha256.ComputeHash(bytes);

            return Convert.ToHexString(hash);
        }

        public bool Verify(string value, string hashedValue)
        {
            var hash = Hash(value);

            return CryptographicOperations.FixedTimeEquals(
                Convert.FromHexString(hash),
                Convert.FromHexString(hashedValue));
        }
    }
}
