using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Infrastructure.Redis.Models
{
    public class RedisPlatformAdminSession
    {
        public Guid SessionId { get; set; }

        public Guid PlatformAdminId { get; set; }

        public bool IsRevoked { get; set; }

        public bool IsPlatformAdminActive { get; set; }

        public DateTime ExpiresAt { get; set; }
    }
}
