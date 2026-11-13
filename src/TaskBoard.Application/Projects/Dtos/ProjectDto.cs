namespace TaskBoard.Application.Projects.Dtos;

public record ProjectDto(
    int Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    int OwnerId,
    int TaskCount
);