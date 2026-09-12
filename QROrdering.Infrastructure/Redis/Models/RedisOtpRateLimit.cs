using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Infrastructure.Redis.Models
{
    public class RedisOtpRateLimit
    {
        //Giới hạn tổng số lần gửi OTP trong một khoảng thời gian
        public int Count { get; set; }
    }
}
