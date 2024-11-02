using Infrastructure.Database;
using Infrastructure.Database.Entities;
using LinqToDB;
using MeShopV1.Bascket;
using System.Collections.Immutable;

namespace MeShopV1.Cart;

public class CartService(IDataConnection db)
{
    private readonly IDataConnection _db = db;

    public async Task<ImmutableArray<ItemResponse>> GetItems(Guid userId, CancellationToken cancellationToken)
    {
        return await _db.Items
            .Where(x => x.UserId == userId)
            .Select(x => new ItemResponse(x.ProductId, x.Quantity))
            .ToImmutableArrayAsync(cancellationToken);
    }

    public async Task<bool> AddItem(Guid userId, ItemRequest addedItem, CancellationToken cancellationToken)
    {
        var result = await _db.Items
            .InsertOrUpdateAsync(() => new Item
            {
                UserId = userId,
                ProductId = addedItem.ProductId,
                Quantity = addedItem.Quantity
            },
            x => new Item
            {
                UserId = x.UserId,
                ProductId = x.ProductId,
                Quantity = x.Quantity + addedItem.Quantity
            }, cancellationToken);

        return result == 1;
    }

    public async Task<bool> RemoveItem(Guid userId, ItemRequest removedItem, CancellationToken cancellationToken)
    {
        var result = await _db.Items
            .Where(x => x.UserId == userId && x.ProductId == removedItem.ProductId)
            .Set(x => x.Quantity, x => x.Quantity > removedItem.Quantity ? x.Quantity - removedItem.Quantity : 0)
            .UpdateWithOutputAsync(cancellationToken);

        if (result.Length == 0)
        {
            return false;
        }

        if (result[0].Inserted.Quantity <= 0)
        {
            await _db.Items
                .Where(x => x.UserId == userId && x.ProductId == removedItem.ProductId)
                .DeleteAsync(cancellationToken);
        }

        return true;
    }
}