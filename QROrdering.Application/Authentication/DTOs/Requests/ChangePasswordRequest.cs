using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Application.Authentication.DTOs.Requests
{
    public class ChangePasswordRequest
    {
        public string VerificationToken { get; set; } = default!;

        public string CurrentPassword { get; set; } = default!;

        public string NewPassword { get; set; } = default!;

        public string ConfirmPassword { get; set; } = default!;
    }
}
