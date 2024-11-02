namespace MeShopV1.Shipments;

public record ShipmentRequest(
    string Carrier, 
    ShipmentUnit[] ShipmentUnits, 
    ShipmentRequestRecipient Recipient, 
    ShipmentRequestAddress Address);

public record ShipmentUnit(string Upc, double Width, double Height, double Depth, double Weight);

public record ShipmentRequestRecipient(string Name, string Phone);

public record ShipmentRequestAddress(
    string AddressLine1,
    string AddressLine2,
    string City,
    string State,
    string ZipCode,
    string Country);
