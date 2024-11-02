using Infrastructure.Database.Entities;

namespace MeShopV1.Catalog;

public record ProductRequest(
    string Title, 
    string Description, 
    string Upc, 
    string Brand, 
    double Price, 
    Guid CategoryId,
    double Width,
    double Height,
    double Depth,
    double Weight);

public record ProductsFilter(
    string[] CategoriesNames,
    string[] BrandsNames, 
    double? MinPrice, 
    double? MaxPrice);