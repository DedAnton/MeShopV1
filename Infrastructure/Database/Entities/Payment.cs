using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Entities;

public record Payment
{
    public required Guid Id { get; set; }
    public required string ExternalId { get; init; }
    public required Guid OrderId { get; init; }
    public required double Amount { get; init; }
}

public static class PaymentEntityConfigurator
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("Payments");
            entity.HasKey(x => x.Id);
        });
    }
}