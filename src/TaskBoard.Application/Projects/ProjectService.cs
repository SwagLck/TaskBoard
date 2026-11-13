using System.Net.Cache;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Application.Common.Exceptions;
using TaskBoard.Application.Common.Interfaces;
using TaskBoard.Application.Projects.Dtos;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Application.Projects;

public class ProjectService : IProjectService
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<CreateProjectRequest> _createValidator;
    private readonly IValidator<UpdateProjectRequest> _updateValidator;

    public ProjectService(
        IAppDbContext db,
        ICurrentUserService currentUser,
        IValidator<CreateProjectRequest> createValidator,
        IValidator<UpdateProjectRequest> updateValidator
    )
    {
        _db = db;
        _currentUser = currentUser;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<List<ProjectDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        return await _db.Projects
            .AsNoTracking()
            .Where(p => p.OwnerId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ProjectDto(
                p.Id, p.Name, p.Description, p.CreatedAt, p.OwnerId, p.Tasks.Count
            )).ToListAsync(cancellationToken);
    }

    public async Task<ProjectDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var project = await GetOwnedProjectAsync(id, cancellationToken);
        return await ToDtoAsync(project.Id, cancellationToken);
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectRequest request, CancellationToken cancellationToken)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        var project = new Project
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            OwnerId = _currentUser.UserId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Projects.Add(project);
        await _db.SaveChangesAsync(cancellationToken);

        return new ProjectDto(project.Id, project.Name, project.Description,
         project.CreatedAt, project.OwnerId, 0);
    }

    public async Task<ProjectDto> UpdateAsync(int id, UpdateProjectRequest request, CancellationToken cancellationToken)
    {
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        var project = await GetOwnedProjectAsync(id, cancellationToken);
        project.Name = request.Name.Trim();
        project.Description = request.Description?.Trim();

        await _db.SaveChangesAsync(cancellationToken);
        return await ToDtoAsync(project.Id, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var project = await GetOwnedProjectAsync(id, cancellationToken);

        _db.Projects.Remove(project);
        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task<Project> GetOwnedProjectAsync(int id, CancellationToken cancellationToken)
    {
        var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new NotFoundException(nameof(Project), id);

        if (project.OwnerId != _currentUser.UserId)
            throw new ForbiddenException();

        return project;
    }

    private async Task<ProjectDto> ToDtoAsync(int id, CancellationToken cancellationToken)
    {
        return await _db.Projects
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProjectDto(
                p.Id, p.Name, p.Description, p.CreatedAt, p.OwnerId, p.Tasks.Count
            )).FirstAsync(cancellationToken);
    }
}