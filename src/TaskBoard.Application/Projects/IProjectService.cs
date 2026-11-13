using TaskBoard.Application.Projects.Dtos;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Application.Projects;

public interface IProjectService
{
    Task<List<ProjectDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<ProjectDto> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<ProjectDto> CreateAsync(CreateProjectRequest request, CancellationToken cancellationToken);
    Task<ProjectDto> UpdateAsync(int id, UpdateProjectRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}