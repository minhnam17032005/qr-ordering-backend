using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Application.Platform.Authentication.DTOs.Responses
{
    public class PlatformAdminLoginResponse
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string AccessToken { get; set; } = null!;
    }
}
