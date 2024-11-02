using Infrastructure.Database;
using Infrastructure.Database.Entities;
using LinqToDB;
using MeShopV1.Orders;
using System.Transactions;

namespace MeShopV1.Shipments;

public class ShipmentService(IDataConnection db, CarrierClient carrierClient)
{
    private readonly IDataConnection _db = db;
    private readonly CarrierClient _carrierClient = carrierClient;

    public async Task CreateShipment(Guid orderId, OrderRequestShipmentInfo shipmentInfo, CancellationToken cancellationToken)
    {
        using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var recipientId = Guid.NewGuid();
        var recipientResult = await _db.Recipients
            .InsertAsync(() => new Recipient
            {
                Id = recipientId,
                Name = shipmentInfo.Recipient.Name,
                Phone = shipmentInfo.Recipient.Phone,
            }, cancellationToken);

        var addressId = Guid.NewGuid();
        var addressResult = await _db.Addresses
            .InsertAsync(() => new Address
            {
                Id = addressId,
                AddressLine1 = shipmentInfo.Address.AddressLine1,
                AddressLine2 = shipmentInfo.Address.AddressLine2,
                City = shipmentInfo.Address.City,
                State = shipmentInfo.Address.State,
                ZipCode = shipmentInfo.Address.ZipCode,
                Country = shipmentInfo.Address.Country,
            }, cancellationToken);

        var shipmentId = Guid.NewGuid();
        var shipmentResult = await _db.Shipments
            .InsertAsync(() => new Shipment
            {
                Id = shipmentId,
                OrderId = orderId,
                Carrier = "FedEx", //shipmentInfo.Carrier,
                TrackNumber = string.Empty,
                RecipientId = recipientId,
                AddressId = addressId
            }, cancellationToken);

        if (recipientResult != 1 || addressResult != 1 || shipmentResult != 1)
        {
            return;
        }

        var reservedUnits = await _db.Units
            .InnerJoin(_db.Products, (u, p) => u.ProductId == p.Id, (u, p) => new 
            { 
                p.Upc, 
                p.Width, 
                p.Height, 
                p.Depth, 
                p.Weight, 
                u.ReservedForOrderId 
            })
            .Where(x => x.ReservedForOrderId == orderId)
            .ToListAsync(cancellationToken);

        var shipmentUnits = reservedUnits
            .Select(x => new ShipmentUnit(x.Upc, x.Width, x.Height, x.Depth, x.Weight))
            .ToArray();
        var recipient = new ShipmentRequestRecipient(shipmentInfo.Recipient.Name, shipmentInfo.Recipient.Phone);
        var address = new ShipmentRequestAddress(
            shipmentInfo.Address.AddressLine1,
            shipmentInfo.Address.AddressLine2,
            shipmentInfo.Address.City,
            shipmentInfo.Address.State,
            shipmentInfo.Address.ZipCode,
            shipmentInfo.Address.Country);
        var shipmentRequest = new ShipmentRequest(shipmentInfo.Carrier, shipmentUnits, recipient, address);

        var trackNumber = await _carrierClient.CreateShipment(shipmentRequest, cancellationToken);

        transactionScope.Complete();

        var trackNumberUpdateResult = await _db.Shipments
            .Where(x => x.Id == shipmentId)
            .Set(x => x.TrackNumber, trackNumber)
            .UpdateAsync(cancellationToken);

        if (trackNumberUpdateResult != 1)
        {
            //logging here
        }
    }
}
