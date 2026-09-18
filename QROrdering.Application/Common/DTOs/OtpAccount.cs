using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QROrdering.Application.Common.Enums;

namespace QROrdering.Application.Common.DTOs
{
    public class OtpAccount
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = null!;

        public OtpAccountType AccountType { get; set; }
    }
}
