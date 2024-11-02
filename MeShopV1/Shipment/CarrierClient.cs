using MeShopV1.Payments;

namespace MeShopV1.Shipments;

public class CarrierClient
{
    public async Task<string> CreateShipment(ShipmentRequest shipment, CancellationToken cancellationToken)
    {
        await Task.Delay(2000, cancellationToken);

        return Guid.NewGuid().ToString("d");
    }
}
