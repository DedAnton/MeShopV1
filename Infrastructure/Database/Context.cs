using Infrastructure.Database.Entities;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

internal class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        UserEntityConfigurator.Apply(modelBuilder);
        ProductEntityConfigurator.Apply(modelBuilder);
        CategoryEntityConfigurator.Apply(modelBuilder);
        ItemEntityConfigurator.Apply(modelBuilder);
        UnitEntityConfigurator.Apply(modelBuilder);
        OrderEntityConfigurator.Apply(modelBuilder);
        LineEntityConfigurator.Apply(modelBuilder);
        PaymentEntityConfigurator.Apply(modelBuilder);
        ShipmentEntityConfigurator.Apply(modelBuilder);
    }
}

public interface IDataConnection : IDataContext
{
    public ITable<IdentityUser> Users { get; }
    public ITable<Product> Products { get; }
    public ITable<Category> Categories { get; }
    public ITable<Item> Items { get; }
    public ITable<Unit> Units { get; }
    public ITable<Order> Orders { get; }
    public ITable<Line> Lines { get; }
    public ITable<Payment> Payments { get; }
    public ITable<Shipment> Shipments { get; }
    public ITable<Recipient> Recipients { get; }
    public ITable<Address> Addresses { get; }

    Task<BulkCopyRowsCopied> BulkInsertAsync<T>(IEnumerable<T> source, CancellationToken cancellationToken) where T : class;
}

public class ApplicationDataConnection : DataConnection, IDataConnection
{
    public ITable<IdentityUser> Users => this.GetTable<IdentityUser>();
    public ITable<Product> Products => this.GetTable<Product>();
    public ITable<Category> Categories => this.GetTable<Category>();
    public ITable<Item> Items => this.GetTable<Item>();
    public ITable<Unit> Units => this.GetTable<Unit>();
    public ITable<Order> Orders => this.GetTable<Order>();
    public ITable<Line> Lines => this.GetTable<Line>();
    public ITable<Payment> Payments => this.GetTable<Payment>();
    public ITable<Shipment> Shipments => this.GetTable<Shipment>();
    public ITable<Recipient> Recipients => this.GetTable<Recipient>();
    public ITable<Address> Addresses => this.GetTable<Address>();

    public ApplicationDataConnection(DataOptions options)
        : base(options)
    {

    }

    public Task<BulkCopyRowsCopied> BulkInsertAsync<T>(IEnumerable<T> source, CancellationToken cancellationToken) where T : class
        => this.BulkCopyAsync(source, cancellationToken);
}
