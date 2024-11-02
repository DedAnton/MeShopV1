using Infrastructure.Identity;

namespace MeShopV1.Orders;

public class OrderApi
{
    public static void Map(WebApplication app)
    {
        var group = app.MapGroup("/orders")
            .WithOpenApi()
            .WithTags("Orders");

        group.MapGet("/", async (UserInfo userInfo, OrderService service, CancellationToken cancellationToken) => 
        {
            return await service.GetOrders(userInfo.Id, cancellationToken);
        });

        group.MapPost("/", async (OrderRequest addedOrder, UserInfo userInfo, OrderService service, CancellationToken cancellationToken) => 
        {
            var isOrderAdded = await service.AddOrder(userInfo.Id, addedOrder, cancellationToken);

            return isOrderAdded ? Results.Ok() : Results.Conflict("Order not added");
        });
    }
}
