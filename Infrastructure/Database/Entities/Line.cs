using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Entities;

public record Line
{
    public required Guid OrderId { get; init; }
    public required Guid ProductId { get; init; }
    public required int Quantity { get; init; }
    public required double UnitPrice { get; init; }
}

public static class LineEntityConfigurator
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Line>(entity =>
        {
            entity.ToTable("Lines");
            entity.HasKey(x => new { x.OrderId, x.ProductId });
        });
    }
}