using Infrastructure.Identity;

namespace MeShopV1.Cart;

public static class CartApi
{
    public static void Map(WebApplication app)
    {
        var group = app.MapGroup("/cart")
            .RequireAuthorization()
            .WithOpenApi()
            .WithTags("Cart");

        group.MapGet("/items", async (UserInfo userInfo, CartService service, CancellationToken cancellationToken) => 
        {
            return await service.GetItems(userInfo.Id, cancellationToken);
        });

        group.MapPost("/items", async (ItemRequest addedItem, UserInfo userInfo, CartService service, CancellationToken cancellationToken) => 
        {
            var isItemAdded = await service.AddItem(userInfo.Id, addedItem, cancellationToken);

            return isItemAdded ? Results.Ok() : Results.Conflict("Item not added");
        });

        group.MapDelete("/items", async (Guid ProductId, uint Quantity, UserInfo userInfo, CartService service, CancellationToken cancellationToken) => 
        {
            var isItemRemoved = await service.RemoveItem(userInfo.Id, new(ProductId, Quantity), cancellationToken);

            return isItemRemoved ? Results.Ok() : Results.Conflict("Item not removed");
        });
    }
}
