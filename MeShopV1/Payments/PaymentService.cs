using Infrastructure.Database;
using Infrastructure.Database.Entities;
using LinqToDB;
using System.Transactions;

namespace MeShopV1.Payments;

public class PaymentService(IDataConnection db, PaymentClient paymentClient)
{
    private readonly IDataConnection _db = db;
    private readonly PaymentClient _paymentClient = paymentClient;

    public async Task RequestPayment(Guid orderId, CardInfo cardInfo, CancellationToken cancellationToken)
    {
        using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var reservedUnits = await _db.Units
            .Where(x => x.ReservedForOrderId == orderId)
            .ToListAsync(cancellationToken);

        var orderLines = await _db.Lines
            .Where(x => x.OrderId == orderId)
            .ToDictionaryAsync(x => x.ProductId, cancellationToken);

        var paymentAmount = reservedUnits.Sum(x => orderLines[x.ProductId].UnitPrice);

        var externalPaymentId = await _paymentClient.AuthorizePayment(
            new PaymentRequest(paymentAmount, cardInfo), 
            cancellationToken);

        var result = await _db.Payments
            .InsertAsync(() => new Payment
            {
                Id = Guid.NewGuid(),
                ExternalId = externalPaymentId,
                OrderId = orderId,
                Amount = paymentAmount
            }, cancellationToken);

        if (result != 1)
        {
            return;
        }

        await _paymentClient.AcceptPayment(externalPaymentId, cancellationToken);

        transactionScope.Complete();
    }
}


