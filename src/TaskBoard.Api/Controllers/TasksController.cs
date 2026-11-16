using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using TaskBoard.Application.Common.Models;
using TaskBoard.Application.TaskItems;
using TaskBoard.Application.TaskItems.Dtos;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Api.Controllers;

[ApiController]
[Authorize]
[Route("api")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _tasks;

    public TasksController(ITaskService tasks) => _tasks = tasks;

    [HttpGet("projects/{projectId:int}/tasks")]
    public async Task<ActionResult<PagedResult<TaskDto>>> GetByProject(
        int projectId, [FromQuery] TaskListQuery query, CancellationToken cancellationToken
    ) => Ok(await _tasks.GetByProjectAsync(projectId, query, cancellationToken));

    [HttpPost("projects/{projectId:int}/tasks")]
    public async Task<ActionResult<TaskDto>> Create(
        int projectId, CreateTaskRequest request, CancellationToken cancellationToken
    )
    {
        var created = await _tasks.CreateAsync(projectId, request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new {id = created.Id}, created);
    }

    [HttpGet("tasks/{id:int}")]
    public async Task<ActionResult<TaskDto>> GetById(int id, CancellationToken cancellationToken)
        => Ok(await _tasks.GetByIdAsync(id, cancellationToken));

    [HttpPut("tasks/{id:int}")]
    public async Task<ActionResult<TaskDto>> Update(int id, UpdateTaskRequest request, CancellationToken cancellationToken)
        => Ok(await _tasks.UpdateAsync(id, request, cancellationToken));

    [HttpPatch("tasks/{id:int}/status")]
    public async Task<ActionResult<TaskDto>> UpdateStatus(int id, UpdateTaskStatusRequest reqeust, CancellationToken cancellationToken)
        => Ok(await _tasks.UpdateStatusAsync(id, reqeust, cancellationToken));

    [HttpDelete("tasks/{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _tasks.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}