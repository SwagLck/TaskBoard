using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TaskBoard.Application.Auth;
using TaskBoard.Application.Comments;
using TaskBoard.Application.Projects;
using TaskBoard.Application.TaskItems;

namespace TaskBoard.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<ICommentService, CommentService>();

        return services;
    }
}