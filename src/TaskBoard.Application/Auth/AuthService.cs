using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Application.Auth.Dtos;
using TaskBoard.Application.Common.Exceptions;
using TaskBoard.Application.Common.Interfaces;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Application.Auth;

public class AuthService : IAuthService
{
    private readonly IAppDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenGenerator _tokens;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<LoginRequest> _loginValidator;

    public AuthService(
        IAppDbContext db,
        IPasswordHasher hasher,
        IJwtTokenGenerator tokens,
        IValidator<RegisterRequest> registerValidator,
        IValidator<LoginRequest> loginValidator
    )
    {
        _db = db;
        _hasher = hasher;
        _tokens = tokens;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }


    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        await _registerValidator.ValidateAndThrowAsync(request, cancellationToken);

        var email = NormalizeEmail(request.Email);

        if(await _db.Users.AnyAsync(u => u.Email == email, cancellationToken))
            throw new ConflictException("A user with this email already exists.");

        var user = new User
        {
            Email = email,
            PasswordHash = _hasher.Hash(request.Password),
            DisplayName = request.DisplayName.Trim()
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);

        return BuildResponse(user);
    }


    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        await _loginValidator.ValidateAndThrowAsync(request, cancellationToken);

        var email = NormalizeEmail(request.Email);
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if(user is null || !_hasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password");

        return BuildResponse(user);
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private AuthResponse BuildResponse(User user)
    {
        var (token, expiresAt) = _tokens.Generate(user);
        return new AuthResponse(token, expiresAt, user.Id, user.Email, user.DisplayName);
    }
}