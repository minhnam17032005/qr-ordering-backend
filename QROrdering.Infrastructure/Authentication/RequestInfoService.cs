using Microsoft.AspNetCore.Http;
using QROrdering.Application.Authentication.Interfaces;
using UAParser;

namespace QROrdering.Infrastructure.Authentication
{
    public class RequestInfoService : IRequestInfoService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestInfoService(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string IpAddress =>
            _httpContextAccessor.HttpContext?
                .Connection
                .RemoteIpAddress?
                .ToString()
            ?? string.Empty;

        public string UserAgent =>
            _httpContextAccessor.HttpContext?
                .Request
                .Headers["User-Agent"]
                .ToString()
            ?? string.Empty;

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
