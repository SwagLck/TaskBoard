using TaskBoard.Domain.Enums;

namespace TaskBoard.Application.TaskItems.Dtos;

public class TaskListQuery
{
    public TaskItemStatus? Status {get; set;}
    public int? AssigneeId {get; set;}
    public int Page {get; set;} = 1;
    public int PageSize {get; set;} = 20;
}