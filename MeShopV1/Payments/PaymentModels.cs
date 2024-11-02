namespace MeShopV1.Payments;

public record CardInfo(
    string CardNumber,
    DateOnly ExpiredOn,
    string Cvv,
    string CardholderName);

public record PaymentRequest(
    double Amount,
    CardInfo CardInfo);
