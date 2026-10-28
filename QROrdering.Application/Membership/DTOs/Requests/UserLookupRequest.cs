using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QROrdering.Application.Membership.Enums;

namespace QROrdering.Application.Membership.DTOs.Requests
{
    public class UserLookupRequest
    {
        public UserLookupType SearchType { get; set; }

        public string Keyword { get; set; } = null!;
    }
}
