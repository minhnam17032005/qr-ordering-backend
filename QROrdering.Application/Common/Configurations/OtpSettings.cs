using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Application.Common.Configurations
{
    public class OtpSettings
    {
        public int Length { get; set; }

        // Thời gian OTP có hiệu lực (phút)
        public int ExpiredMinutes { get; set; }

        // Thời gian Verification Token có hiệu lực (phút)
        public int VerificationExpiredMinutes { get; set; }

        // Thời gian chờ trước khi được phép gửi lại OTP (giây)
        public int ResendCooldownSeconds { get; set; }

        // Số lần nhập OTP sai tối đa
        public int MaxVerifyAttempts { get; set; }

        // Số lần gửi OTP tối đa trong một khoảng thời gian
        public int MaxSendPerWindow { get; set; }

        // Khoảng thời gian giới hạn số lần gửi OTP (phút)
        public int RateLimitWindowMinutes { get; set; }
    }
}
