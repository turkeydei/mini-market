namespace Application.Interfaces;

public interface IVnPayService
{
    string CreatePaymentUrl(int orderId, decimal amount, string orderInfo, string returnUrl, string ipAddress);
    bool VerifyPaymentSignature(Dictionary<string, string> vnpayData, string vnpHashSecret);
    Dictionary<string, string> ParseReturnUrl(string queryString);
}

