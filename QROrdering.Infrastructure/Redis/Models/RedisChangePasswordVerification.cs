using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Infrastructure.Redis.Models
{
    //quy định Value lưu trong Redis.
    public class RedisChangePasswordVerification
    {
        public string TokenHash { get; set; } = default!;
    }
}
