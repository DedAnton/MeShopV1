namespace MeShopV1.Bascket;

public record ItemRequest(Guid ProductId, uint Quantity);

public record ItemResponse(Guid ProductId, uint Quantity);
