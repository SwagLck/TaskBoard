using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Application.Comments.Dtos;
using TaskBoard.Application.Common.Exceptions;
using TaskBoard.Application.Common.Interfaces;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Application.Comments;

public class CommentService : ICommentService
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<CreateCommentRequest> _validator;

    public CommentService(
        IAppDbContext db,
        ICurrentUserService currentUser,
        IValidator<CreateCommentRequest> validator
    )
    {
        _db = db;
        _currentUser = currentUser;
        _validator = validator;
    }

    public async Task<List<CommentDto>> GetByTaskAsync(int taskId, CancellationToken cancellationToken)
    {
        await EnsureTaskAccessAsync(taskId, cancellationToken);

        return await _db.Comments
            .AsNoTracking()
            .Where(c => c.TaskItemId == taskId)
            .OrderBy(c => c.CreatedAt)
            .ThenBy(c => c.Id)
            .Select(c => new CommentDto(c.Id, c.Text, c.CreatedAt, c.AuthorId, c.Author.DisplayName))
            .ToListAsync(cancellationToken);
    }

    public async Task<CommentDto> AddAsync(int taskId, CreateCommentRequest request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        await EnsureTaskAccessAsync(taskId, cancellationToken);

        var comment = new Comment
        {
            Text = request.Text.Trim(),
            TaskItemId = taskId,
            AuthorId = _currentUser.UserId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Comments.Add(comment);
        await _db.SaveChangesAsync(cancellationToken);

        return await _db.Comments
            .AsNoTracking()
            .Where(c => c.Id == comment.Id)
            .Select(c => new CommentDto(c.Id, c.Text, c.CreatedAt, c.AuthorId, c.Author.DisplayName))
            .FirstAsync(cancellationToken);
    }

    private async Task EnsureTaskAccessAsync(int taskId, CancellationToken cancellationToken)
    {
        var ownerId = await _db.Tasks
            .Where(t => t.Id == taskId)
            .Select(t => (int?)t.Project.OwnerId)
            .FirstOrDefaultAsync(cancellationToken);

        if (ownerId is null)
            throw new NotFoundException(nameof(TaskItem), taskId);

        if (ownerId != _currentUser.UserId)
            throw new ForbiddenException();
    }
}