using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Infrastructure.Redis
{
    public static class RedisKeys
    {
        /// <summary>
        /// JWT blacklist key
        /// </summary>
        public static string BlacklistToken(string jti)
            => $"qroordering:blacklist:jwt:{jti}";

        /// <summary>
        /// User session cache key
        /// </summary>
        public static string Session(Guid sessionId)
            => $"qroordering:session:{sessionId}";
    }
}
