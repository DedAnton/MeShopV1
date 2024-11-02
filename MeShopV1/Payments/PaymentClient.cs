namespace MeShopV1.Payments;

public class PaymentClient
{
    public async Task<string> AuthorizePayment(PaymentRequest payment, CancellationToken cancellationToken)
    {
        await Task.Delay(2000, cancellationToken);

        return Guid.NewGuid().ToString("d");
    }

    public async Task AcceptPayment(string paymentId, CancellationToken cancellationToken)
    {
        await Task.Delay(2000, cancellationToken);
    }
}
