using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Infrastructure.Helpers
{
    // Sinh mã OTP ngẫu nhiên gồm 6 chữ số
    public static class OtpGenerator
    {
        public static string GenerateOtp()
        {
            return RandomNumberGenerator
                .GetInt32(100000, 1000000)
                .ToString();
        }
    }
}
