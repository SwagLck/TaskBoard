using TaskBoard.Domain.Entities;

namespace TaskBoard.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAt) Generate(User user);
}