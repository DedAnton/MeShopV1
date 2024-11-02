using Infrastructure.Database;
using Infrastructure.Database.Entities;
using LinqToDB;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Threading;

namespace MeShopV1.Catalog;

public class CatalogService(IDataConnection db)
{
    private readonly IDataConnection _db = db;

    public async Task<ImmutableArray<Product>> SearchProducts(ProductsFilter filter, CancellationToken cancellationToken)
    {
        var searchQuery = _db.Products
            .LoadWith(x => x.Category)
            .AsQueryable();

        if (filter.CategoriesNames.Length > 0)
        {
            searchQuery = searchQuery
                .Where(x => filter.CategoriesNames.Contains(x.Category!.Name));
        }

        if (filter.BrandsNames.Length > 0)
        {
            searchQuery = searchQuery
                .Where(x => filter.BrandsNames.Contains(x.Brand));
        }

        if (filter.MinPrice is not null)
        {
            searchQuery = searchQuery
                .Where(x => filter.MinPrice <= x.Price);
        }

        if (filter.MaxPrice is not null)
        {
            searchQuery = searchQuery
                .Where(x => x.Price <= filter.MaxPrice);
        }

        var products = await searchQuery.ToListAsync();

        return ImmutableArray.Create(CollectionsMarshal.AsSpan(products));
    }

    public async Task<Product?> GetProduct(Guid id, CancellationToken cancellationToken)
    {
        return await _db.Products
            .LoadWith(x => x.Category)
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Product?> GetProductByUpc(string upc, CancellationToken cancellationToken)
    {
        return await _db.Products
            .LoadWith(x => x.Category)
            .Where(x => x.Upc == upc)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> AddProduct(ProductRequest newProduct, CancellationToken cancellationToken)
    {
        var result = await _db.Products
            .InsertAsync(() => new Product
            {
                Id = Guid.NewGuid(),
                Title = newProduct.Title,
                Description = newProduct.Description,
                Upc = newProduct.Upc,
                Brand = newProduct.Brand,
                Price = newProduct.Price,
                CategoryId = newProduct.CategoryId,
                Width = newProduct.Width,
                Height = newProduct.Height,
                Depth = newProduct.Depth,
                Weight = newProduct.Weight
            }, cancellationToken);

        return result == 1;
    }

    public async Task<bool> UpdateProduct(Guid id, ProductRequest updatedProduct, CancellationToken cancellationToken)
    {
        var result = await _db.Products
            .Where(x => x.Id == id)
            .Set(x => x.Title, updatedProduct.Title)
            .Set(x => x.Description, updatedProduct.Description)
            .Set(x => x.Upc, updatedProduct.Upc)
            .Set(x => x.CategoryId, updatedProduct.CategoryId)
            .Set(x => x.Brand, updatedProduct.Brand)
            .Set(x => x.Price, updatedProduct.Price)
            .UpdateAsync(cancellationToken);

        return result == 1;
    }

    public async Task<Category?> GetCategoryTree(string rootCategoryName, CancellationToken cancellationToken)
    {
        var recursiveCte = _db
            .GetCte<Category>(categoryCte => _db.Categories
                .Where(c => c.Name == rootCategoryName)
                .Concat(_db.Categories
                    .InnerJoin(
                        categoryCte,
                        (c, r) => c.ParentId == r.Id,
                        (c, r) => new Category
                        {
                            Id = c.Id,
                            Name = c.Name,
                            ParentId = c.ParentId
                        })));

        var result = await recursiveCte.ToListAsync(cancellationToken);
        if (result.Count == 0)
        {
            return null;
        }

        var rootCategory = result.First(x => x.Name.Equals(rootCategoryName, StringComparison.OrdinalIgnoreCase));

        return BuildCategoryTree(rootCategory, result);
    }

    public Category BuildCategoryTree(Category category, List<Category> other)
    {
        var childs = other
            .Where(x => x.ParentId == category.Id)
            .Select(x => BuildCategoryTree(x, other))
            .ToList();

        return category with { Subcategories = childs };
    }
}