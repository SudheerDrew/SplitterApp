using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models;

namespace server.Services
{
    public class GroupService
    {
        private readonly AppDbContext _context;

        public GroupService(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Add Members to a Group
        public async Task<bool> AddMembersToGroup(int groupId, List<int> userIds)
        {
            var group = await _context.Groups.Include(g => g.Members).FirstOrDefaultAsync(g => g.GroupID == groupId);
            if (group == null)
                return false;

            foreach (var userId in userIds)
            {
                // Skip if the user is already a member
                if (group.Members.Any(m => m.UserID == userId))
                    continue;

                _context.GroupMembers.Add(new GroupMember
                {
                    GroupID = groupId,
                    UserID = userId
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        // ✅ Get Group with Members
        public async Task<Group?> GetGroupWithMembers(int groupId)
        {
            return await _context.Groups
                .Include(g => g.Members)
                .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(g => g.GroupID == groupId);
        }

        // ✅ Get Groups for a User
        public async Task<List<Group>> GetGroupsForUser(int userId)
        {
            return await _context.Groups
                .Where(g => g.Members.Any(m => m.UserID == userId))
                .Include(g => g.Members)
                .ToListAsync();
        }
    }
}
