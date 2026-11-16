using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskBoard.Application.Comments;
using TaskBoard.Application.Comments.Dtos;

namespace TaskBoard.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/tasks/{taskId:int}/comments")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _comments;

    public CommentsController(ICommentService comments) => _comments = comments;

    [HttpGet]
    public async Task<ActionResult<List<CommentDto>>> GetAll(int taskId, CancellationToken cancellationToken)
        => Ok(await _comments.GetByTaskAsync(taskId, cancellationToken));

    [HttpPost]
    public async Task<ActionResult<CommentDto>> Add(int taskId, CreateCommentRequest request, CancellationToken cancellationToken)
    {
        var created = await _comments.AddAsync(taskId, request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, created);
    }
}