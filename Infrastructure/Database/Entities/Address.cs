using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Entities;

public record Address
{
    public required Guid Id { get; init; }
    public required string AddressLine1 { get; init; }
    public required string AddressLine2 { get; init; }
    public required string City { get; init; }
    public required string State { get; init; }
    public required string ZipCode { get; init; }
    public required string Country { get; init; }
}

public static class AddressEntityConfigurator
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.ToTable("Addresses");
            entity.HasKey(x => x.Id);
        });
    }
}