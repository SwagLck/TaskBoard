namespace TaskBoard.Application.Auth.Dtos;

public record RegisterRequest(string Email, string Password, string DisplayName);
public record LoginRequest(string Email, string Password);
public record AuthResponse(string Token, DateTime ExpiresAt, int UserId, string Email, string DisplayName);