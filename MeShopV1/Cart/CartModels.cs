namespace MeShopV1.Cart;

public record ItemRequest(Guid ProductId, uint Quantity);

public record ItemResponse(Guid ProductId, uint Quantity);
