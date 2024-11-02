using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Entities;

public record Recipient
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Phone { get; init; }
}

public static class RecipientEntityConfigurator
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Recipient>(entity =>
        {
            entity.ToTable("Recipients");
            entity.HasKey(x => x.Id);
        });
    }
}