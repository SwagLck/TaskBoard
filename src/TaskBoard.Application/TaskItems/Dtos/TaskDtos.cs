using Microsoft.EntityFrameworkCore.Diagnostics;
using TaskBoard.Domain.Enums;

namespace TaskBoard.Application.TaskItems.Dtos;

public record TaskDto(
    int Id,
    string Title,
    string? Description,
    TaskItemStatus Status,
    TaskPriority Priority,
    DateTime? DueDate,
    DateTime CreatedAt,
    int ProjectId,
    int? AssigneeId,
    string? AssigneeName,
    int CommentCount
);

public record CreateTaskRequest(
    string Title,
    string? Description,
    TaskPriority Priority,
    DateTime? DueDate,
    int? AssigneeId
);

public record UpdateTaskRequest(
    string Title,
    string? Description,
    TaskPriority Priority,
    DateTime? DueDate,
    int? AssigneeId
);

public record UpdateTaskStatusRequest(TaskItemStatus Status);