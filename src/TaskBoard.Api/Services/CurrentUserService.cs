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
            var header = _accessor.HttpContext?.Request.Headers["X-User-ID"].FirstOrDefault();
            return int.TryParse(header, out var id) ? id : 1;
        }
    }
}