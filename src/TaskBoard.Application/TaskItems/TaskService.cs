using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Application.Common.Exceptions;
using TaskBoard.Application.Common.Interfaces;
using TaskBoard.Application.Common.Models;
using TaskBoard.Application.TaskItems.Dtos;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Application.TaskItems;

public class TaskService : ITaskService
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<CreateTaskRequest> _createValidator;
    private readonly IValidator<UpdateTaskRequest> _updateValidator;
    private readonly IValidator<UpdateTaskStatusRequest> _statusValidator;
    private readonly IValidator<TaskListQuery> _queryValidator;

    public TaskService(
        IAppDbContext db,
        ICurrentUserService currentUser,
        IValidator<CreateTaskRequest> createValidator,
        IValidator<UpdateTaskRequest> updateValidator,
        IValidator<UpdateTaskStatusRequest> statusValidator,
        IValidator<TaskListQuery> queryValidator
    )
    {
        _db = db;
        _currentUser = currentUser;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _statusValidator = statusValidator;
        _queryValidator = queryValidator;
    }

    private static readonly Expression<Func<TaskItem, TaskDto>> ToDto = t => new TaskDto(
        t.Id,
        t.Title,
        t.Description,
        t.Status,
        t.Priority,
        t.DueDate,
        t.CreatedAt,
        t.ProjectId,
        t.AssigneeId,
        t.Assignee != null ? t.Assignee.DisplayName : null,
        t.Comments.Count
    );

    public async Task<PagedResult<TaskDto>> GetByProjectAsync(
        int projectId, TaskListQuery query, CancellationToken cancellationToken
    )
    {
        await _queryValidator.ValidateAndThrowAsync(query, cancellationToken);
        await EnsureProjectOwnedAsync(projectId, cancellationToken);

        IQueryable<TaskItem> tasks = _db.Tasks
                .AsNoTracking()
                .Where(t => t.ProjectId == projectId);

        if (query.Status.HasValue)
            tasks = tasks.Where(t => t.Status == query.Status.Value);

        if (query.AssigneeId.HasValue)
            tasks = tasks.Where(t => t.AssigneeId == query.AssigneeId.Value);

        var totalCount = await tasks.CountAsync(cancellationToken);

        var items = await tasks
            .OrderByDescending(t => t.CreatedAt)
            .ThenByDescending(t => t.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(ToDto)
            .ToListAsync(cancellationToken);

        return new PagedResult<TaskDto>(items, query.Page, query.PageSize, totalCount);
    }

    public async Task<TaskDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        await GetOwnedTaskAsync(id, cancellationToken);
        return await LoadDtoAsync(id, cancellationToken);
    }

    public async Task<TaskDto> CreateAsync(int projectId, CreateTaskRequest request, CancellationToken cancellationToken)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);
        await EnsureProjectOwnedAsync(projectId, cancellationToken);
        await EnsureAssigneeExistsAsync(request.AssigneeId, cancellationToken);

        var task = new TaskItem
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            Priority = request.Priority,
            DueDate = request.DueDate,
            AssigneeId = request.AssigneeId,
            ProjectId = projectId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Tasks.Add(task);
        await _db.SaveChangesAsync(cancellationToken);

        return await LoadDtoAsync(task.Id, cancellationToken);
    }

    public async Task<TaskDto> UpdateAsync(int id, UpdateTaskRequest request, CancellationToken cancellationToken)
    {
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        var task = await GetOwnedTaskAsync(id, cancellationToken);
        await EnsureAssigneeExistsAsync(request.AssigneeId, cancellationToken);

        task.Title = request.Title.Trim();
        task.Description = request.Description?.Trim();
        task.Priority = request.Priority;
        task.DueDate = request.DueDate;
        task.AssigneeId = request.AssigneeId;

        await _db.SaveChangesAsync(cancellationToken);
        return await LoadDtoAsync(id, cancellationToken);
    }

    public async Task<TaskDto> UpdateStatusAsync(int id, UpdateTaskStatusRequest request, CancellationToken cancellationToken)
    {
        await _statusValidator.ValidateAndThrowAsync(request, cancellationToken);

        var task = await GetOwnedTaskAsync(id, cancellationToken);
        task.Status = request.Status;

        await _db.SaveChangesAsync(cancellationToken);
        return await LoadDtoAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var task = await GetOwnedTaskAsync(id, cancellationToken);

        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync(cancellationToken);
    }

    // Biski pagalbiniu metodu po apacia

    private async Task EnsureProjectOwnedAsync(int projectId, CancellationToken cancellationToken)
    {
        var ownerId = await _db.Projects
            .Where(p => p.Id == projectId)
            .Select(p => (int?)p.OwnerId)
            .FirstOrDefaultAsync(cancellationToken);

        if(ownerId is null)
            throw new NotFoundException(nameof(Project), projectId);

        if(ownerId != _currentUser.UserId)
            throw new ForbiddenException();
    }

    private async Task<TaskItem> GetOwnedTaskAsync(int id, CancellationToken cancellationToken)
    {
        var task = await _db.Tasks
            .Include(t => t.Project)   
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
            ?? throw new NotFoundException(nameof(TaskItem), id);

        if (task.Project.OwnerId != _currentUser.UserId)
            throw new ForbiddenException();

        return task;
    }

    private async Task EnsureAssigneeExistsAsync(int? assigneeId, CancellationToken cancellationToken)
    {
        if (assigneeId is null) return;

        if(!await _db.Users.AnyAsync(u => u.Id == assigneeId, cancellationToken))
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(CreateTaskRequest.AssigneeId), "Assignee does not exist")
            });
        }
    }

    private async Task<TaskDto> LoadDtoAsync(int id, CancellationToken cancellationToken)
    {
        return await _db.Tasks
            .AsNoTracking()
            .Where(t => t.Id == id)
            .Select(ToDto)
            .FirstAsync(cancellationToken);
    }
}