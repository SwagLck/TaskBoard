using System.Transactions;
using TaskBoard.Domain.Enums;

namespace TaskBoard.Domain.Entities;

public class TaskItem
{
    public int Id {get; set;}
    public string Title {get; set;} = string.Empty;
    public string? Description {get; set;}
    public TaskItemStatus Status {get; set;} = TaskItemStatus.ToDo;
    public TaskPriority Priority {get; set;} = TaskPriority.Medium;
    public DateTime? DueDate {get; set;}
    public DateTime CreatedAt {get; set;}

    public int ProjectId {get; set;}
    public Project Project {get; set;} = null!;

    public int? AssigneeId {get; set;}
    public User? Assignee {get; set;}

    public List<Comment> Comments {get; set;} = new();
}