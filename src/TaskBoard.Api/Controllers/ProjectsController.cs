using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Mvc;
using TaskBoard.Application.Projects;
using TaskBoard.Application.Projects.Dtos;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Api.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projects;

    public ProjectsController(IProjectService projects) => _projects = projects;

    [HttpGet]
    public async Task<ActionResult<List<ProjectDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await _projects.GetAllAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectDto>> GetById(int id, CancellationToken cancellationToken)
        => Ok(await _projects.GetByIdAsync(id, cancellationToken));

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create(CreateProjectRequest request, CancellationToken cancellationToken)
    {
        var created = await _projects.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProjectDto>> Update(int id, UpdateProjectRequest request, CancellationToken cancellationToken)
        => Ok(await _projects.UpdateAsync(id, request, cancellationToken));

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ProjectDto>> Delete(int id, CancellationToken cancellationToken)
    {
        await _projects.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
        
}