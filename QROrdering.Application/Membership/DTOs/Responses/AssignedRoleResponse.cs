using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Application.Membership.DTOs.Responses
{
    public class AssignedRoleResponse
    {
        public Guid RoleId { get; set; }

        public string RoleName { get; set; } = null!;
    }
}
