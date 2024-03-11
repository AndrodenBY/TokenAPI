using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TokenAPI.Models;

namespace TokenAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {

        private ApplicationContext _context;
        public CommentController(ApplicationContext context) { _context = context; }

        [HttpGet("GetComments")]
        public ActionResult<IEnumerable<Comment>> Get()
        {
            try
            {
                return Ok(_context.Comments.ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error getting comments: {ex.Message}");
            }
        }

        [HttpPost("AddComments")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> Add(Comment newComment)
        {
            try
            {                    
                    newComment.Description = newComment.Description;

                    _context.Comments.Add(newComment);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction(nameof(Get), new { Description = newComment.Description }, newComment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error adding comment: {ex.Message}");
            }
        }

        [HttpPut("EditComments")]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(Guid id, Comment comment)
        {
            Comment? commentToEdit = _context.Comments.FirstOrDefault(c => c.Id == id);
            if (commentToEdit != null)
                try
                {
                    commentToEdit.Description = comment.Description;
                    commentToEdit.Moderate = comment.Moderate;
                    _context.Comments.Update(commentToEdit);
                    _context.SaveChangesAsync();
                    return CreatedAtAction(nameof(Get), new { Status = "Success", Message = $"Comment '{commentToEdit.Id}' edited." });
                }
                catch (Exception e)
                {
                    return StatusCode(500, $"Error editing comment: {e.Message}");
                }
            else
            {
                return CreatedAtAction(nameof(Get), new { Status = "Error", Message = $"Comment: '{id}' not found." });
            }
        }

        [HttpDelete("DeleteComments")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            Comment? commentToDelete = _context.Comments.FirstOrDefault(c => c.Id == id);
            if (commentToDelete != null)
            {
                try
                {
                    _context.Comments.Remove(commentToDelete);
                    _context.SaveChangesAsync();
                    return CreatedAtAction(nameof(Get), new { Status = "Success", Message = $"Comment '{commentToDelete.Id}' deleted." });
                }
                catch (Exception e)
                {
                    return StatusCode(500, $"Error deleting comment: {e.Message}");
                }
            }
            else
            {
                return CreatedAtAction(nameof(Get), new { Status = "Error", Message = $"Comment: '{id}' not found." });
            }
        }
    }
}
