using MeShopV1.Payments;

namespace MeShopV1.Orders;

public record OrderRequest(OrderRequestLine[] Lines, CardInfo CardInfo, OrderRequestShipmentInfo Shipment);

public record OrderRequestLine(Guid ProductId, int Quantity);

public record OrderRequestShipmentInfo(
    string Carrier, 
    OrderRequestShipmentInfoRecipient Recipient, 
    OrderRequestShipmentInfoAddess Address);

public record OrderRequestShipmentInfoRecipient(string Name, string Phone);

public record OrderRequestShipmentInfoAddess(
    string AddressLine1,
    string AddressLine2,
    string City,
    string State,
    string ZipCode,
    string Country);