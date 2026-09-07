using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Application.Authentication.Interfaces
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        string Username { get; }
        Guid SessionId { get; }
        string Jti { get; }
        long IssuedAt { get; }
        string ExpiredAtString { get; }
        List<string> Roles { get; }
        bool IsAuthenticated { get; }
    }
}
