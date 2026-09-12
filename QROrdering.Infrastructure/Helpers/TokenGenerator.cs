using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Infrastructure.Helpers
{
    // Sinh chuỗi 64 ký tự hex đủ mạnh để làm Verification Token
    public static class TokenGenerator
    {
        public static string GenerateSecureToken()
        {
            return Convert.ToHexString(
                RandomNumberGenerator.GetBytes(32));
        }
    }
}