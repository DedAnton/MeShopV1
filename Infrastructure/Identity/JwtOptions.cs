namespace Infrastructure.Auth;

public record JwtOptions
{
    public required string Key { get; init; }
}
