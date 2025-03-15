using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GroupsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GroupsController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Get All Groups for the Logged-In User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Group>>> GetGroups()
        {
            var userId = int.Parse(User.FindFirst("UserID")!.Value); // Get user ID from JWT claims

            var groups = await _context.Groups
                .Where(g => g.Members!.Any(m => m.UserID == userId)) // Add '!' to bypass nullability issues
                .Include(g => g.Members!) // Null-forgiving operator
                .ThenInclude(m => m.User) // Include User from GroupMember
                .ToListAsync();

            return Ok(groups);
        }

        // ✅ Get a Specific Group by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Group>> GetGroup(int id)
        {
            var group = await _context.Groups
                .Include(g => g.Members!) // Add '!' here
                .ThenInclude(m => m.User) // Include User from GroupMember
                .FirstOrDefaultAsync(g => g.GroupID == id);

            if (group == null)
                return NotFound();

            return Ok(group);
        }

        // ✅ Create a New Group
        [HttpPost]
        public async Task<ActionResult<Group>> PostGroup([FromBody] Group group)
        {
            // Add the group
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            // Return the created group
            return CreatedAtAction(nameof(GetGroup), new { id = group.GroupID }, group);
        }

        // ✅ Add Members to a Group
        [HttpPost("{id}/add-members")]
        public async Task<IActionResult> AddMembers(int id, [FromBody] List<int> userIds)
        {
            var group = await _context.Groups.Include(g => g.Members!).FirstOrDefaultAsync(g => g.GroupID == id); // Add '!' here
            if (group == null)
                return NotFound(new { message = "Group not found" });

            foreach (var userId in userIds)
            {
                // Check if the user is already a member
                if (group.Members!.Any(m => m.UserID == userId)) // Add '!' here
                    continue;

                _context.GroupMembers.Add(new GroupMember
                {
                    GroupID = group.GroupID,
                    UserID = userId
                });
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Members added successfully" });
        }

        // ✅ Delete a Group by ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var group = await _context.Groups.FindAsync(id);

            if (group == null)
                return NotFound();

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
