using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Application.Membership.DTOs.Requests
{
    public class AddMemberRequest
    {
        public Guid UserId { get; set; }

        public List<Guid> RoleIds { get; set; } = new();
    }
}
