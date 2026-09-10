using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Application.Authentication.DTOs
{
    public class UserSessionResponse
    {
        public Guid SessionId { get; set; }

        public string? DeviceName { get; set; }

        public string? IpAddress { get; set; }

        public string? UserAgent { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? LastAccessAt { get; set; }

        public DateTime ExpiredAt { get; set; }

        public bool IsCurrent { get; set; }
    }
}
