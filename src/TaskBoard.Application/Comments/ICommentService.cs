using TaskBoard.Application.Comments.Dtos;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Application.Comments;

public interface ICommentService
{
    Task<List<CommentDto>> GetByTaskAsync(int taskId, CancellationToken cancellationToken);
    Task<CommentDto> AddAsync(int taskId, CreateCommentRequest request, CancellationToken cancellationToken);
}