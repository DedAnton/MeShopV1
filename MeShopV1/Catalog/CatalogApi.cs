namespace MeShopV1.Catalog;

public static class CatalogApi
{
    public static void Map(WebApplication app)
    {
        var group = app.MapGroup("/catalog")
            .WithOpenApi()
            .WithTags("Catalog");

        group.MapGet("/products/search", async (
            string[] category,
            string[] brand,
            double? minPrice,
            double? maxPrice,
            CatalogService service,
            CancellationToken cancellationToken) =>
        {
            var filter = new ProductsFilter(category, brand, minPrice, maxPrice);

            return await service.SearchProducts(filter, cancellationToken);
        });

        group.MapGet("/products/{id}", async (
            Guid id, 
            CatalogService service, 
            CancellationToken cancellationToken) =>
        {
            var product = await service.GetProduct(id, cancellationToken);

            return product is not null ? Results.Ok(product) : Results.NotFound("Product not found");
        });

        group.MapPost("/products", async (
            ProductRequest newProduct,
            CatalogService service,
            CancellationToken cancellationToken) =>
        {
            var isProductAdded = await service.AddProduct(newProduct, cancellationToken);

            return isProductAdded ? Results.Ok() : Results.Conflict("Product not added");
        });

        group.MapPut("/products/{id}", async (
            Guid id,
            ProductRequest updatedProduct, 
            CatalogService service,
            CancellationToken cancellationToken) =>
        {
            var isProductUpdated = await service.UpdateProduct(id, updatedProduct, cancellationToken);

            return isProductUpdated ? Results.Ok() : Results.Conflict("Product not updated");
        });

        group.MapGet("/products/categories/{rootCategoryName}", async (
            string rootCategoryName,
            CatalogService service, 
            CancellationToken cancellationToken) =>
        {
            var categoryTree = await service.GetCategoryTree(rootCategoryName, cancellationToken);

            return categoryTree is not null ? Results.Ok(categoryTree) : Results.NotFound("Category not found");
        });
    }
}
