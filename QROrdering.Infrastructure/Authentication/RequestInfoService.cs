using Microsoft.AspNetCore.Http;
using QROrdering.Application.Authentication.Interfaces;
using UAParser;

namespace QROrdering.Infrastructure.Authentication
{
    public class RequestInfoService : IRequestInfoService
    {
        // Truy cập thông tin HTTP request hiện tại
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestInfoService(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Lấy địa chỉ IP của client
        public string IpAddress =>
            _httpContextAccessor.HttpContext?
                .Connection
                .RemoteIpAddress?
                .ToString()
            ?? string.Empty;

        // Lấy User-Agent của client
        public string UserAgent =>
            _httpContextAccessor.HttpContext?
                .Request
                .Headers["User-Agent"]
                .ToString()
            ?? string.Empty;

        // Phân tích hệ điều hành và trình duyệt
        public string DeviceName
        {
            get
            {
                var client = Parser.GetDefault().Parse(UserAgent);

                var os = client.OS.Family;
                var browser = client.UA.Family;

                return $"{os} - {browser}";
            }
        }
    }
}
