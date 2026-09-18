using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Application.Platform.Authentication.DTOs.Responses
{
    public class PlatformAdminRefreshResponse
    {
        public string AccessToken { get; set; } = null!;
    }
}
