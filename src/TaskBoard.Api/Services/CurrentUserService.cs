using System.Security.Claims;
using TaskBoard.Application.Common.Exceptions;
using TaskBoard.Application.Common.Interfaces;

namespace TaskBoard.Api.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUserService(IHttpContextAccessor accessor) => _accessor = accessor;

    public int UserId
    {
        get
        {
            var value = _accessor.HttpContext?.User.FindFirstValue("sub");
            return int.TryParse(value, out var id) ? id : throw new UnauthorizedException();
        }
    }
}