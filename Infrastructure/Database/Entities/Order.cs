using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Entities;

public record Order
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }

    public ICollection<Line>? OrderLines { get; init; }
    public ICollection<Unit>? ReservedUnits { get; init; }
    public Payment? Payment { get; init; }
    public Shipment? Shipment { get; init; }
}

public static class OrderEntityConfigurator
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Orders");
            entity.HasKey(x => x.Id);

            entity
                .HasMany(x => x.OrderLines)
                .WithOne()
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity
                .HasMany(x => x.ReservedUnits)
                .WithOne()
                .HasForeignKey(x => x.ReservedForOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(x => x.Payment)
                .WithOne()
                .HasForeignKey<Payment>(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(x => x.Shipment)
                .WithOne()
                .HasForeignKey<Shipment>(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
