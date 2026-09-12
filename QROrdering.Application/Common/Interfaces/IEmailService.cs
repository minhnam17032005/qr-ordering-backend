using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Application.Common.Interfaces
{
    public interface IEmailService
    {
        // Gửi email OTP
        Task SendOtpAsync(
            string email,
            string subject,
            string message,
            string otp);
    }
}

