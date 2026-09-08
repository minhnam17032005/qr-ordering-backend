using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Infrastructure.Configurations
{
    public class SessionCacheSettings
    {
        public int RedisExpirationMinutes { get; set; }

        public int MemoryExpirationSeconds { get; set; }
    }
}
