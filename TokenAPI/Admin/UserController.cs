using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TokenAPI.Models;

namespace TokenAPI.Admin
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        
        private ApplicationContext _context;
        public UserController(ApplicationContext context) { _context = context; }

        [HttpGet("GetUsers")]
        [Authorize(Roles = "Admin")]
        public ActionResult<IEnumerable<User>> Get()
        {
            try
            {
                return Ok(_context.Users.ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error getting users: {ex.Message}");
            }
        }

        [HttpPut("MakeAdmin")]
        [Authorize(Roles = "Admin")]
        public IActionResult MakeAdmin(Guid id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) 
            {
                return NotFound();
            }
            user.Role = "Admin";
            _context.Users.Update(user);
            _context.SaveChanges();
            return Ok($"{user.Id} is now an admin");
        }

        [HttpDelete("DeleteUser")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(Guid id) 
        {
            Models.User? userToDelete = _context.Users.FirstOrDefault(u => u.Id == id);
            if (userToDelete != null)
            {
                try
                {
                    _context.Users.Remove(userToDelete);
                    _context.SaveChangesAsync();
                    return CreatedAtAction(nameof(Get), new { Status = "Success", Message = $"User '{userToDelete.Id}' deleted." });
                }
                catch (Exception e)
                {
                    return StatusCode(500, $"Error deleting user: {e.Message}");
                }
            }
            else
            {
                return CreatedAtAction(nameof(Get), new { Status = "Error", Message = $"Comment: '{id}' not found." });
            }
        }
    }
}
