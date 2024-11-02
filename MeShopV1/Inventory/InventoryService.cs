using Infrastructure.Database;
using Infrastructure.Database.Entities;
using LinqToDB;
using MeShopV1.Orders;
using System.Collections.Immutable;
using System.Transactions;

namespace MeShopV1.Inventory;

public class InventoryService(IDataConnection db)
{
    private readonly IDataConnection _db = db;

    public async Task<ImmutableArray<UnitResponse>> GetUnits(CancellationToken cancellationToken)
    {
        return await _db.Units
            .Select(x => new UnitResponse(x.Id, x.ProductId, x.ReservedForOrderId))
            .ToImmutableArrayAsync(cancellationToken);
    }

    public async Task<bool> AddUnit(Guid productId, CancellationToken cancellationToken)
    {
        var result = await _db.Units
            .InsertAsync(() => new Unit
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                ReservedForOrderId = null
            }, cancellationToken);

        return result == 1;
    }

    public async Task<bool> RemoveUnit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _db.Units
            .Where(x => x.Id == id && x.ReservedForOrderId == null)
            .DeleteAsync(cancellationToken);

        return result == 1;
    }

    public async Task<bool> ReserveUnits(List<UnitsReserve> reserves, Guid orderId, CancellationToken cancellationToken)
    {
        var productIds = reserves
            .Select(x => x.ProductId)
            .ToArray();

        var unitsToReserve = _db.Units
            .Where(u => productIds.Contains(u.ProductId) && u.ReservedForOrderId == null)
            .Select(u => new
            {
                u.Id,
                u.ProductId,
                RowNum = Sql.Ext.RowNumber().Over().PartitionBy(u.ProductId).OrderBy(u.Id).ToValue()
            })
            .AsCte("UnitsToReserve");

        var selectedUnits = unitsToReserve
            .Join(reserves, u => u.ProductId, p => p.ProductId, (u, p) => new { u.Id, u.RowNum, p.RequestedQuantity })
            .Where(x => x.RowNum <= x.RequestedQuantity)
            .AsCte("SelectedUnits");

        var result = await _db.Units
            .Where(u => selectedUnits.Any(su => su.Id == u.Id))
            .Set(u => u.ReservedForOrderId, orderId)
            .UpdateAsync(cancellationToken);

        return result > 0;
    }
}