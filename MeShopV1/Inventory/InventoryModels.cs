using Infrastructure.Database.Entities;

namespace MeShopV1.Inventory;

public record UnitRequest(string Upc);

public record UnitResponse(Guid Id, Guid ProductId, Guid? ReservedForOrderId);

public record UnitsReserve(Guid ProductId, int RequestedQuantity);
