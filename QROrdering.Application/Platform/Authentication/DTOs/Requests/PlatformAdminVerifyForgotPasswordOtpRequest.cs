using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Application.Platform.Authentication.DTOs.Requests
{
    public class PlatformAdminVerifyForgotPasswordOtpRequest
    {
        public string Email { get; set; } = null!;

        public string Otp { get; set; } = null!;
    }
}
