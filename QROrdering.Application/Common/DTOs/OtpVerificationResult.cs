using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Application.Common.DTOs
{
    public class OtpVerificationResult
    {
        public string VerificationToken { get; set; } = null!;

        public DateTime ExpiredAt { get; set; }
    }
}
