using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Infrastructure.Membership.DTOs.Requests
{
    public class UpdateMemberRolesRequest
    {
        public List<Guid> RoleIds { get; set; } = new();
    }
}
