using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Infrastructure.Redis.Models
{
    //quy định Value lưu trong Redis.
    public class RedisEmailOtp
    {
        public string OtpHash { get; set; } = default!;

        // Số lần nhập sai OTP
        public int FailedAttempts { get; set; }

    }
}
