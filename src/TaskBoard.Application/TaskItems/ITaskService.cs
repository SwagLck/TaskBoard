using TaskBoard.Application.Common.Models;
using TaskBoard.Application.TaskItems.Dtos;

namespace TaskBoard.Application.TaskItems;

public interface ITaskService
{
    Task<PagedResult<TaskDto>> GetByProjectAsync(int projectId, TaskListQuery query, CancellationToken cancellationToken);
    Task<TaskDto> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<TaskDto> CreateAsync(int projectId, CreateTaskRequest request, CancellationToken cancellationToken);
    Task<TaskDto> UpdateAsync(int id, UpdateTaskRequest request, CancellationToken cancellationToken);
    Task<TaskDto> UpdateStatusAsync(int id, UpdateTaskStatusRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}