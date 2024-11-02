using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Entities;

public record IdentityUser
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public required string PasswordHash { get; init; }
}

public static class UserEntityConfigurator
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityUser>(entity =>
        {
            entity.ToTable("Users", Constants.DatabaseInfastructureSchema);
            entity.HasKey(x => x.Id);
        });
    }
}
