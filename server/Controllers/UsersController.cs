using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Get All Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users
                .Include(u => u.GroupMemberships)
                .ThenInclude(gm => gm.Group) // Include associated groups
                .ToListAsync();

            return Ok(users);
        }

        // ✅ Get User By ID
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.GroupMemberships)
                .ThenInclude(gm => gm.Group) // Include associated groups
                .FirstOrDefaultAsync(u => u.UserID == id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // ✅ Update User
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, [FromBody] User updatedUser)
        {
            if (id != updatedUser.UserID)
                return BadRequest();

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
                return NotFound();

            // Update only allowed fields
            existingUser.Name = updatedUser.Name;
            existingUser.Email = updatedUser.Email;

            _context.Entry(existingUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // ✅ Delete User
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.GroupMemberships)
                .FirstOrDefaultAsync(u => u.UserID == id);

            if (user == null)
                return NotFound();

            // Remove related GroupMemberships before deleting the user
            _context.GroupMembers.RemoveRange(user.GroupMemberships);
            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Helper: Check if User Exists
        private bool UserExists(int id)
        {
            return _context.Users.Any(u => u.UserID == id);
        }
    }
}
