using Microsoft.AspNetCore.Mvc;
using TaskBoard.Application.Auth;
using TaskBoard.Application.Auth.Dtos;

namespace TaskBoard.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
        => Ok(await _auth.RegisterAsync(request, cancellationToken));

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
        => Ok(await _auth.LoginAsync(request, cancellationToken));
}