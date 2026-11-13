using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TaskBoard.Application.Projects;

namespace TaskBoard.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<IProjectService, ProjectService>();

        return services;
    }
}