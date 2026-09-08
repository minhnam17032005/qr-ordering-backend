using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Application.Authentication.DTOs
{
    public class MemberRoleResponse
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; } = null!;
    }
}
