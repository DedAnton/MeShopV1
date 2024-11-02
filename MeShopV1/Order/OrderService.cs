using Hangfire;
using Infrastructure.Database;
using LinqToDB;
using MeShopV1.Inventory;
using System.Collections.Immutable;
using System.Transactions;
using Infrastructure.Database.Entities;
using MeShopV1.Payments;
using MeShopV1.Shipments;

namespace MeShopV1.Orders;

public class OrderService(IDataConnection db, IBackgroundJobClient backgroundJobClient)
{
    private readonly IDataConnection _db = db;
    private readonly IBackgroundJobClient _backgroundJobs = backgroundJobClient;

    public async Task<ImmutableArray<Order>> GetOrders(Guid userId, CancellationToken cancellationToken)
    {
        return await _db.Orders
            .LoadWith(x => x.OrderLines)
            .LoadWith(x => x.ReservedUnits)
            .LoadWith(x => x.Payment)
            .LoadWith(x => x.Shipment)
            .Where(x => x.UserId == userId)
            .ToImmutableArrayAsync(cancellationToken);
    }

    public async Task<bool> AddOrder(Guid userId, OrderRequest orderRequest, CancellationToken cancellationToken)
    {
        using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var orderId = Guid.NewGuid();

        var reserves = orderRequest.Lines
            .Select(x => new UnitsReserve(x.ProductId, x.Quantity))
            .ToList();
        var reservationJobId = _backgroundJobs.Enqueue<InventoryService>(x => x.ReserveUnits(reserves, orderId, CancellationToken.None));
        var paymentJobId = _backgroundJobs.ContinueJobWith<PaymentService>(reservationJobId,
            x => x.RequestPayment(orderId, orderRequest.CardInfo, CancellationToken.None));
        _backgroundJobs.ContinueJobWith<ShipmentService>(paymentJobId, x => x.CreateShipment(orderId, orderRequest.Shipment, CancellationToken.None));

        var orderProductIds = orderRequest.Lines.Select(x => x.ProductId).ToList();
        var orderProductsPrices = await _db.Products
            .Where(x => orderProductIds.Contains(x.Id))
            .Select(x => new { x.Id, x.Price })
            .ToDictionaryAsync(k => k.Id, cancellationToken);

        var orderResult = await _db.Orders
            .InsertAsync(() => new Order
            {
                Id = orderId,
                UserId = userId,
            }, cancellationToken);

        var lines = orderRequest.Lines
            .Select(x => new Line
            {
                OrderId = orderId,
                ProductId = x.ProductId,
                Quantity = x.Quantity,
                UnitPrice = orderProductsPrices[x.ProductId].Price
            });

        var lineResult = await _db.BulkInsertAsync(lines, cancellationToken);
        var isSuccess = orderResult == 1 && lineResult.RowsCopied == lines.Count();

        if (isSuccess == false)
        {
            return false;
        }

        transactionScope.Complete();

        return true;
    }
}