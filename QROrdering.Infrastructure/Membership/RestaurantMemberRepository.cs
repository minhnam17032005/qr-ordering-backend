
using Microsoft.EntityFrameworkCore;
using QROrdering.Application.Membership.Interfaces;
using QROrdering.Domain.Entities.Membership;
using QROrdering.Domain.Enums;
using QROrdering.Infrastructure.Persistence;

namespace QROrdering.Infrastructure.Membership
{
    public class RestaurantMemberRepository : IRestaurantMemberRepository
    {
        private readonly QROrderingDbContext _context;

        public RestaurantMemberRepository(
            QROrderingDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(
            RestaurantMember restaurantMember)
        {
            await _context.RestaurantMembers.AddAsync(
                restaurantMember);
        }

        public async Task<bool> IsActiveAdminAsync(
            Guid userId,
            Guid restaurantId)
        {
            return await _context.RestaurantMembers
                .AsNoTracking()
                .AnyAsync(member =>
                    member.UserId == userId &&
                    member.RestaurantId == restaurantId &&
                    member.IsActive &&
                    member.User.IsActive &&
                    member.MemberRoles.Any(memberRole =>
                        memberRole.RestaurantId == restaurantId &&
                        memberRole.Role.RestaurantId == restaurantId &&
                        memberRole.Role.IsActive &&
                        memberRole.Role.Type == RoleType.Admin));
        }

        public async Task<int> CountByRestaurantAsync(
            Guid restaurantId)
        {
            return await _context.RestaurantMembers
                .CountAsync(member =>
                    member.RestaurantId == restaurantId);
        }

        public async Task<List<RestaurantMember>>
            GetPagedByRestaurantAsync(
                Guid restaurantId,
                int page,
                int pageSize)
        {
            return await _context.RestaurantMembers
                .AsNoTracking()
                .Where(member =>
                    member.RestaurantId == restaurantId)
                .Include(member => member.User)
                .Include(member => member.MemberRoles)
                    .ThenInclude(memberRole => memberRole.Role)
                .OrderByDescending(member => member.CreatedAt)
                .ThenBy(member => member.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<RestaurantMember?>
            GetByIdInRestaurantAsync(
                Guid restaurantId,
                Guid memberId)
        {
            return await _context.RestaurantMembers
                .AsNoTracking()
                .Where(member =>
                    member.Id == memberId &&
                    member.RestaurantId == restaurantId)
                .Include(member => member.User)
                .Include(member => member.MemberRoles)
                    .ThenInclude(memberRole => memberRole.Role)
                .FirstOrDefaultAsync();
        }


        public async Task<RestaurantMember?> GetByUserIdAndRestaurantIdAsync(
            Guid userId,
            Guid restaurantId)
        {
            return await _context.RestaurantMembers
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.RestaurantId == restaurantId);
        }

        public async Task<int> CountActiveAdminsAsync(Guid restaurantId)
        {
            return await _context.RestaurantMembers
                .AsNoTracking()
                .Where(member =>
                    member.RestaurantId == restaurantId &&
                    member.IsActive &&
                    member.User.IsActive &&
                    member.MemberRoles.Any(memberRole =>
                        memberRole.RestaurantId == restaurantId &&
                        memberRole.Role.RestaurantId == restaurantId &&
                        memberRole.Role.IsActive &&
                        memberRole.Role.Type == RoleType.Admin))
                .CountAsync();
        }

        public async Task<List<RestaurantMember>>GetActiveMembershipsWithDetailsByUserIdAsync(Guid userId)
        {
            return await _context.RestaurantMembers
                .AsNoTracking()
                .Where(member =>
                    member.UserId == userId &&
                    member.IsActive &&
                    member.User.IsActive &&
                    member.Restaurant.IsActive)
                .Include(member => member.Restaurant)
                .Include(member => member.MemberRoles)
                    .ThenInclude(memberRole => memberRole.Role)
                .OrderBy(member => member.Restaurant.Name)
                .ThenBy(member => member.RestaurantId)
                .ToListAsync();
        }

    }
}
