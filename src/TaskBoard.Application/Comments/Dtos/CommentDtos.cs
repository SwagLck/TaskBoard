namespace TaskBoard.Application.Comments.Dtos;

public record CommentDto(int Id, string Text, DateTime CreatedAt, int AuthorId, string AuthorName);

public record CreateCommentRequest(string Text);