using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Entities;

public record Product
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string Upc { get; init; }
    public required string Brand { get; init; }
    public required double Price { get; init; }
    public required Guid CategoryId { get; init; }
    public required double Width { get; init; }
    public required double Height { get; init; }
    public required double Depth { get; init; }
    public required double Weight { get; init; }

    public Category? Category { get; init;}
}

public static class ProductEntityConfigurator
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Upc).IsUnique();

            //entity.OwnsOne(x => x.Dimentions);

            entity
                .HasOne(x => x.Category)
                .WithMany()
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        });
    }
}