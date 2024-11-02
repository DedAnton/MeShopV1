using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Entities;

public record Item
{
    public required Guid UserId { get; init; }
    public required Guid ProductId { get; init; }
    public required uint Quantity { get; init; }
}

public static class ItemEntityConfigurator
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>(entity =>
        {
            entity.ToTable("Items");
            entity.HasKey(x => new { x.UserId, x.ProductId });
        });
    }
}