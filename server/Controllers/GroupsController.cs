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
        public async Task<ActionResult<IEnumerable<GroupDto>>> GetGroups()
        {
            var userId = int.Parse(User.FindFirst("UserID")!.Value); // Get user ID from JWT claims

            var groups = await _context.Groups
                .Where(g => g.Members!.Any(m => m.UserID == userId))
                .Include(g => g.Members!)
                .ThenInclude(m => m.User)
                .ToListAsync();

            // Map to DTOs
            var groupDtos = groups.Select(g => new GroupDto
            {
                GroupID = g.GroupID,
                GroupName = g.GroupName,
                Members = g.Members!.Select(m => new MemberDto
                {
                    UserID = m.UserID,
                    Name = m.User!.Name
                }).ToList()
            });

            return Ok(groupDtos);
        }
        
// ✅ Get a Specific Group by ID (with Expenses and Balance Summary)
[HttpGet("{id}")]
public async Task<ActionResult<GroupDto>> GetGroup(int id)
{
    // Extract the logged-in user's ID from JWT claims
    var userId = int.Parse(User.FindFirst("UserID")!.Value);

    // Fetch group details, including members, expenses, and payer information
    var group = await _context.Groups
        .Include(g => g.Members!) // Include group members
        .ThenInclude(m => m.User) // Include user details for members
        .Include(g => g.Expenses) // Include expenses
        .ThenInclude(e => e.PaidBy) // Include the user who paid for each expense
        .FirstOrDefaultAsync(g => g.GroupID == id && g.Members!.Any(m => m.UserID == userId));

    if (group == null)
        return NotFound(new { message = "Group not found or access denied" });

    // Calculate the balance summary for each group member
    var balanceSummary = group.Members!.Select(m => new BalanceSummaryDto
    {
        UserID = m.UserID,
        Name = m.User!.Name,
        Owes = group.Expenses!
            .Where(e => e.UserID != m.UserID) // Filter expenses not paid by this user
            .Sum(e => e.Amount / group.Members.Count), // Each member owes an equal share
        OwedByGroup = group.Expenses!
            .Where(e => e.UserID == m.UserID) // Filter expenses paid by this user
            .Sum(e => e.Amount) // Total amount this user has paid for others
    }).ToList();

    // Map the group and its details into a DTO
    var groupDto = new GroupDto
    {
        GroupID = group.GroupID,
        GroupName = group.GroupName,
        Members = group.Members!.Select(m => new MemberDto
        {
            UserID = m.UserID,
            Name = m.User!.Name
        }).ToList(),
        Expenses = group.Expenses!.Select(e => new ExpenseDto
        {
            ExpenseID = e.ExpenseID,
            Description = e.Description,
            Amount = e.Amount,
            CreatedAt = e.CreatedAt,
            PaidBy = e.PaidBy?.Name ?? "Unknown" // Safely handle null PaidBy
        }).ToList(),
        BalanceSummary = balanceSummary
    };

    return Ok(groupDto);
}



        // ✅ Create a New Group
        [HttpPost]
        public async Task<ActionResult<GroupDto>> PostGroup([FromBody] Group group)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserID")!.Value);

                // Add the creator as a member of the group
                group.Members = new List<GroupMember>
                {
                    new GroupMember { UserID = userId }
                };

                _context.Groups.Add(group);
                await _context.SaveChangesAsync();

                // Fetch the newly created group
                var createdGroup = await _context.Groups
                    .Include(g => g.Members!)
                    .ThenInclude(m => m.User)
                    .FirstOrDefaultAsync(g => g.GroupID == group.GroupID);

                if (createdGroup == null)
                    return StatusCode(500, new { message = "Failed to retrieve created group." });

                // Map to DTO
                var groupDto = new GroupDto
                {
                    GroupID = createdGroup.GroupID,
                    GroupName = createdGroup.GroupName,
                    Members = createdGroup.Members!.Select(m => new MemberDto
                    {
                        UserID = m.UserID,
                        Name = m.User!.Name
                    }).ToList()
                };

                return CreatedAtAction(nameof(GetGroup), new { id = group.GroupID }, groupDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        // ✅ Add Members to a Group
        [HttpPost("{id}/add-members")]
        public async Task<IActionResult> AddMembers(int id, [FromBody] List<int> userIds)
        {
            var group = await _context.Groups.Include(g => g.Members!).FirstOrDefaultAsync(g => g.GroupID == id);
            if (group == null)
                return NotFound(new { message = "Group not found" });

            // Check if all user IDs exist
            var validUserIds = await _context.Users
                .Where(u => userIds.Contains(u.UserID))
                .Select(u => u.UserID)
                .ToListAsync();

            // Find invalid user IDs
            var invalidUserIds = userIds.Except(validUserIds).ToList();
            if (invalidUserIds.Any())
            {
                return BadRequest(new { message = $"The following UserIDs do not exist: {string.Join(", ", invalidUserIds)}" });
            }

            foreach (var userId in validUserIds)
            {
                if (group.Members!.Any(m => m.UserID == userId))
                    continue; // Skip if the user is already a member

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
