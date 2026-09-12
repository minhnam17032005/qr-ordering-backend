using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Application.Authentication.DTOs.Responses
{
    public class VerifyOtpResponse
    {
        public string VerificationToken { get; set; } = string.Empty;

        public DateTime ExpiredAt { get; set; }
    }
}
