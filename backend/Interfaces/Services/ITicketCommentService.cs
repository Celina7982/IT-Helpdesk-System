using IThelpdesk.DTOs;

namespace IThelpdesk.Interfaces.Services
{
    public interface ITicketCommentService
    {
        Task<List<CommentResponseDto>> GetCommentsAsync(
    int ticketId,
    string userRole);

        Task<CommentResponseDto> AddCommentAsync(
            int ticketId,
            CreateCommentDto dto,
            int loggedInUserId,
            string authorName,
            string authorRole);

        Task<CommentResponseDto> UpdateCommentAsync(
            int ticketId,
            int commentId,
            UpdateCommentDto dto,
            int loggedInUserId,
            string userRole);
    }
}