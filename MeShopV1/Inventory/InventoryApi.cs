using MeShopV1.Catalog;

namespace MeShopV1.Inventory;

public static class InventoryApi
{
    public static void Map(WebApplication app)
    {
        var group = app.MapGroup("/inventory")
            .WithOpenApi()
            .WithTags("Inventory");

        group.MapGet("/units", async (
            InventoryService service, 
            CancellationToken cancellationToken) => 
        { 
            return await service.GetUnits(cancellationToken);
        });

        group.MapPost("/units", async (
            UnitRequest addedUnit, 
            InventoryService inventoryService,
            CatalogService catalogService,
            CancellationToken cancellationToken) => 
        {
            var product = await catalogService.GetProductByUpc(addedUnit.Upc, cancellationToken);
            if (product is null)
            {
                return Results.NotFound("Product not found");
            }

            var isUnitAdded = await inventoryService.AddUnit(product.Id, cancellationToken);

            return isUnitAdded ? Results.Ok() : Results.Conflict("Unit not added");
        });

        group.MapDelete("/units/{id}", async (
            Guid id, 
            InventoryService service, 
            CancellationToken cancellationToken) => 
        {
            var isUnitRemoved = await service.RemoveUnit(id, cancellationToken);

            return isUnitRemoved ? Results.Ok() : Results.Conflict("Unit not removed");
        });
    }
}
