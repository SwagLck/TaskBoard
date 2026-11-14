using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TaskBoard.Application.Auth;
using TaskBoard.Application.Projects;

namespace TaskBoard.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}