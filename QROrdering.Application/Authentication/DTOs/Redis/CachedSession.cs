using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Application.Authentication.DTOs.Redis
{
    public class CachedSession
    {
        public Guid SessionId { get; set; }

        public Guid UserId { get; set; }

        public bool IsRevoked { get; set; }

        public bool IsUserActive { get; set; }

        public DateTime ExpiredAt { get; set; }
    }
}
