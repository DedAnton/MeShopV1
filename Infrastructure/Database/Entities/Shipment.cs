using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Entities;

public record Shipment
{
    public required Guid Id { get; init; }
    public required Guid OrderId {  get; init; }
    public required string Carrier { get; init; }
    public required string TrackNumber { get; init; }
    public required Guid RecipientId { get; init; }
    public required Guid AddressId { get; init; }

    public Recipient? Recipient { get; init; }
    public Address? Address { get; init; }
}

public static class ShipmentEntityConfigurator
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Shipment>(entity =>
        {
            entity.ToTable("Shipments");
            entity.HasKey(x => x.Id);

            //Not supported in linq2db.EntityFrameworkCore package
            //entity.OwnsOne(x => x.Recipient);
            //entity.OwnsOne(x => x.Address);

            entity
                .HasOne(x => x.Recipient)
                .WithOne()
                .HasForeignKey<Shipment>(x => x.RecipientId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity
                .HasOne(x => x.Address)
                .WithOne()
                .HasForeignKey<Shipment>(x => x.AddressId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        });
    }
}