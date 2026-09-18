using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Application.Platform.Authentication.DTOs.Requests
{
    public class PlatformAdminLoginRequest
    {
        public string Identifier { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
