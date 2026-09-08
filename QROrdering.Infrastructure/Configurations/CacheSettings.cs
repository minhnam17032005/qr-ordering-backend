using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Infrastructure.Configurations
{
    public class CacheSettings
    {
        public PermissionCacheSettings Permission { get; set; } = new();

        public SessionCacheSettings Session { get; set; } = new();
    }
}
