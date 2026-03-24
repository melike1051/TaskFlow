using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskFlow.API.Data;
using TaskFlow.Core.Entities;

namespace TaskFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly TaskFlowDbContext _context;

        public UsersController(TaskFlowDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Select(user => new UserListItemDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Role = user.Role,
                    TaskCount = _context.Tasks.Count(task => task.UserId == user.Id)
                })
                .OrderBy(user => user.Username)
                .ToListAsync();

            return Ok(users);
        }

        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateUserRoleRequest request)
        {
            if (request.Role is not ("Admin" or "User"))
                return BadRequest("Invalid role");

            var user = await _context.Users.FirstOrDefaultAsync(item => item.Id == id);
            if (user == null)
                return NotFound("User not found");

            if (user.Role == "Admin" && request.Role == "User")
            {
                var adminCount = await _context.Users.CountAsync(item => item.Role == "Admin");
                if (adminCount <= 1)
                    return BadRequest("At least one admin user must remain.");
            }

            user.Role = request.Role;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users.FirstOrDefaultAsync(item => item.Id == id);
            if (user == null)
                return NotFound("User not found");

            if (user.Id == currentUserId)
                return BadRequest("You cannot delete your own account.");

            if (user.Role == "Admin")
            {
                var adminCount = await _context.Users.CountAsync(item => item.Role == "Admin");
                if (adminCount <= 1)
                    return BadRequest("At least one admin user must remain.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public sealed class UserListItemDto
        {
            public int Id { get; set; }
            public string Username { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
            public int TaskCount { get; set; }
        }

        public sealed class UpdateUserRoleRequest
        {
            public string Role { get; set; } = "User";
        }
    }
}
