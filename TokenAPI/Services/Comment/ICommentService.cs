using TokenAPI.DTO;
using TokenAPI.Models;

namespace TokenAPI.Services.Comment
{
    public interface ICommentService
    {
        CommentDto? GetModeratedComment(bool moderate);
    }
}
