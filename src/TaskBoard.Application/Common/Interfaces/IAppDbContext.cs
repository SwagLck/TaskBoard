using Microsoft.EntityFrameworkCore;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Application.Common.Interfaces;


public interface IAppDbContext
{
    DbSet<User> Users {get;}
    DbSet<Project> Projects {get;}
    DbSet<TaskItem> Tasks {get;}
    DbSet<Comment> Comments {get;}

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}