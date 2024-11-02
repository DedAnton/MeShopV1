using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Entities;

public record Unit
{
    public required Guid Id { get; init; }
    public required Guid ProductId { get; init; }
    public required Guid? ReservedForOrderId { get; init; }
}

public static class UnitEntityConfigurator
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Unit>(entity =>
        {
            entity.ToTable("Units");
            entity.HasKey(x => x.Id);

            entity
                .HasOne<Product>()
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        });
    }
}