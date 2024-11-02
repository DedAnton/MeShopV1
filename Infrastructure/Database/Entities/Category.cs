using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Entities;

public record Category
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required Guid? ParentId { get; init; }

    public ICollection<Category>? Subcategories { get; init; }
}

public static class CategoryEntityConfigurator
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Categories");
            entity.HasKey(x => x.Id);

            entity
                .HasMany(x => x.Subcategories)
                .WithOne()
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
