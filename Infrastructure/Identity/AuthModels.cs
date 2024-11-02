namespace Infrastructure.Identity;

record RegisterRequest(string Email, string Password);

record LoginRequest(string Email, string Password);

public record UserInfo(Guid Id, string Email);
