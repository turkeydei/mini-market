using System.Security.Cryptography;
using System.Text;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Collections.Specialized;

namespace Application.Services;

public class VnPayService : IVnPayService
{
    private readonly IConfiguration _configuration;
    private readonly string _vnpTmnCode;
    private readonly string _vnpHashSecret;
    private readonly string _vnpUrl;
    private readonly string _vnpReturnUrl;

    public VnPayService(IConfiguration configuration)
    {
        _configuration = configuration;
        
        // Lấy và trim các giá trị để loại bỏ khoảng trắng thừa
        // QUAN TRỌNG: Kiểm tra xem đã copy đúng Terminal Code và Hash Secret chưa
        var tmnCode = configuration["VnPay:TmnCode"]?.Trim() ?? throw new ArgumentNullException("VnPay:TmnCode");
        var hashSecret = configuration["VnPay:HashSecret"]?.Trim() ?? throw new ArgumentNullException("VnPay:HashSecret");
        
        // Kiểm tra xem có giá trị rỗng không
        if (string.IsNullOrWhiteSpace(tmnCode))
            throw new ArgumentException("VnPay:TmnCode cannot be empty");
        if (string.IsNullOrWhiteSpace(hashSecret))
            throw new ArgumentException("VnPay:HashSecret cannot be empty");
        
        _vnpTmnCode = tmnCode;
        _vnpHashSecret = hashSecret;
        _vnpUrl = configuration["VnPay:Url"] ?? throw new ArgumentNullException("VnPay:Url");
        _vnpReturnUrl = configuration["VnPay:ReturnUrl"] ?? throw new ArgumentNullException("VnPay:ReturnUrl");
    }

    public string CreatePaymentUrl(int orderId, decimal amount, string orderInfo, string returnUrl, string ipAddress)
    {
        // Tạo order ID cho VNPay (format: YYYYMMDDHHmmss + orderId)
        var vnpTxnRef = DateTime.Now.ToString("yyyyMMddHHmmss") + orderId;
        
        // Chuyển đổi amount từ VND sang đồng (VNPay yêu cầu số nguyên)
        var vnpAmount = (long)(amount * 100);
        
        // Tạo create date (format: yyyyMMddHHmmss)
        var vnpCreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");
        
        // Xử lý orderInfo - loại bỏ dấu tiếng Việt và ký tự đặc biệt
        // VNPay yêu cầu orderInfo không có ký tự đặc biệt, chỉ chữ số và khoảng trắng
        var cleanOrderInfo = orderInfo ?? "Thanh toan don hang";
        
        // Loại bỏ dấu tiếng Việt (normalize to ASCII)
        cleanOrderInfo = RemoveVietnameseDiacritics(cleanOrderInfo);
        
        // Loại bỏ ký tự đặc biệt, chỉ giữ chữ, số, khoảng trắng
        cleanOrderInfo = System.Text.RegularExpressions.Regex.Replace(cleanOrderInfo, @"[^a-zA-Z0-9\s]", "");
        
        // Giới hạn độ dài orderInfo (VNPay yêu cầu tối đa 255 ký tự)
        if (cleanOrderInfo.Length > 255)
        {
            cleanOrderInfo = cleanOrderInfo.Substring(0, 255);
        }
        
        // Trim và thay nhiều khoảng trắng thành một
        cleanOrderInfo = System.Text.RegularExpressions.Regex.Replace(cleanOrderInfo.Trim(), @"\s+", " ");
        
        if (string.IsNullOrWhiteSpace(cleanOrderInfo))
        {
            cleanOrderInfo = "Thanh toan don hang";
        }
        
        // QUAN TRỌNG: Chuyển IP address sang IPv4
        // VNPay yêu cầu IP address phải là định dạng IPv4
        var vnpIpAddr = GetIPv4Address(ipAddress);
        
        // Tạo request data - KHai báo ĐẦY ĐỦ các tham số bắt buộc
        // QUAN TRỌNG: 
        // - vnp_OrderType không có giá trị "other"
        // - Giá trị hợp lệ: "billpayment" (thanh toán hóa đơn), "topup" (nạp tiền), "fashion" (thời trang), "other" KHÔNG hợp lệ
        // - Có thể thêm vnp_BankCode = "VNBANK" (tùy chọn)
        // - Sắp xếp theo alphabet (KHÔNG bao gồm vnp_SecureHash)
        var vnpParams = new Dictionary<string, string>
        {
            { "vnp_Amount", vnpAmount.ToString() },
            { "vnp_Command", "pay" },
            { "vnp_CreateDate", vnpCreateDate },
            { "vnp_CurrCode", "VND" },
            { "vnp_IpAddr", vnpIpAddr },
            { "vnp_Locale", "vn" },
            { "vnp_OrderInfo", cleanOrderInfo },
            { "vnp_OrderType", "billpayment" }, // "billpayment" = thanh toán hóa đơn (KHÔNG dùng "other")
            { "vnp_ReturnUrl", returnUrl },
            { "vnp_TmnCode", _vnpTmnCode },
            { "vnp_TxnRef", vnpTxnRef },
            { "vnp_Version", "2.1.0" }
            // Có thể thêm: { "vnp_BankCode", "VNBANK" } nếu cần
        };

        // Sắp xếp theo key (alphabet, case-sensitive) để tạo query string
        // QUAN TRỌNG: Phải sử dụng StringComparer.Ordinal để so sánh chính xác
        // Kiểm tra lại: các key phải được sắp xếp đúng thứ tự alphabet
        var sortedParams = vnpParams.OrderBy(x => x.Key, StringComparer.Ordinal).ToList();

        // Debug: Kiểm tra thứ tự sắp xếp
        // Thứ tự đúng phải là: vnp_Amount, vnp_Command, vnp_CreateDate, vnp_CurrCode, vnp_IpAddr, 
        // vnp_Locale, vnp_OrderInfo, vnp_OrderType, vnp_ReturnUrl, vnp_TmnCode, vnp_TxnRef, vnp_Version

        // Tạo query string để tính hash
        // QUAN TRỌNG: Theo code demo VNPay (sortObject function):
        // - Phải ENCODE giá trị TRƯỚC KHI tạo query string để tính hash
        // - Format: key=encoded_value&key=encoded_value (KHÔNG có vnp_SecureHash)
        // - Encode sử dụng encodeURIComponent(value).replace(/%20/g, "+")
        // - Tức là: encode giá trị, sau đó thay %20 thành + cho khoảng trắng
        var queryStringForHash = new StringBuilder();
        for (int i = 0; i < sortedParams.Count; i++)
        {
            if (i > 0)
            {
                queryStringForHash.Append("&");
            }
            // QUAN TRỌNG: ENCODE giá trị TRƯỚC KHI tính hash
            // Encode key và value, thay %20 thành +
            var encodedKey = EncodeValueForHash(sortedParams[i].Key);
            var encodedValue = EncodeValueForHash(sortedParams[i].Value);
            queryStringForHash.Append($"{encodedKey}={encodedValue}");
        }

        // Tạo hash từ query string ĐÃ ĐƯỢC ENCODE (với + thay vì %20)
        var dataForHash = queryStringForHash.ToString();
        var vnpSecureHash = HmacSHA512(_vnpHashSecret, dataForHash);

        // Tạo URL cuối cùng
        // QUAN TRỌNG: 
        // - vnp_SecureHash phải là tham số cuối cùng, KHÔNG được sắp xếp lại
        // - Dùng lại query string đã encode (dataForHash) khi tạo URL
        // - Hash không encode
        var urlBuilder = new StringBuilder();
        urlBuilder.Append(_vnpUrl);
        urlBuilder.Append("?");
        urlBuilder.Append(dataForHash); // Dùng lại query string đã encode
        
        // Thêm vnp_SecureHash ở cuối cùng (KHÔNG sắp xếp, KHÔNG encode)
        urlBuilder.Append("&");
        urlBuilder.Append("vnp_SecureHash");
        urlBuilder.Append("=");
        urlBuilder.Append(vnpSecureHash); // Hash không encode

        return urlBuilder.ToString();
    }

    public bool VerifyPaymentSignature(Dictionary<string, string> vnpayData, string vnpHashSecret)
    {
        // Lấy secure hash từ VNPay response
        var vnpSecureHash = vnpayData.ContainsKey("vnp_SecureHash") ? vnpayData["vnp_SecureHash"] : "";
        
        // Loại bỏ vnp_SecureHash và vnp_SecureHashType khỏi dictionary để tính hash
        var dataToVerify = vnpayData
            .Where(x => x.Key != "vnp_SecureHash" && x.Key != "vnp_SecureHashType")
            .OrderBy(x => x.Key, StringComparer.Ordinal)
            .ToList();

        // Tạo query string ĐÃ ĐƯỢC ENCODE để tính hash
        // QUAN TRỌNG: Phải encode lại các giá trị (giống như khi tạo payment URL)
        // Vì VNPay tạo hash từ giá trị đã encode, nên ta cũng phải encode lại để verify
        var queryString = new StringBuilder();
        for (int i = 0; i < dataToVerify.Count; i++)
        {
            if (i > 0)
            {
                queryString.Append("&");
            }
            // Encode key và value, thay %20 thành + (giống như khi tạo payment URL)
            var encodedKey = EncodeValueForHash(dataToVerify[i].Key);
            var encodedValue = EncodeValueForHash(dataToVerify[i].Value);
            queryString.Append($"{encodedKey}={encodedValue}");
        }

        // Tính hash từ query string ĐÃ ĐƯỢC ENCODE (với + thay vì %20)
        var dataForHash = queryString.ToString();
        var checkSum = HmacSHA512(vnpHashSecret, dataForHash);

        // So sánh hash (case-insensitive)
        return checkSum.Equals(vnpSecureHash, StringComparison.InvariantCultureIgnoreCase);
    }

    public Dictionary<string, string> ParseReturnUrl(string queryString)
    {
        var vnpayData = new Dictionary<string, string>();
        
        if (string.IsNullOrEmpty(queryString))
            return vnpayData;

        // Xử lý query string - VNPay có thể dùng + hoặc %20 cho khoảng trắng
        var queries = queryString.TrimStart('?').Split('&');
        foreach (var query in queries)
        {
            var parts = query.Split('=', 2); // Split tối đa 2 parts để xử lý giá trị có dấu =
            if (parts.Length == 2)
            {
                // Decode key và value
                var key = Uri.UnescapeDataString(parts[0].Replace("+", " "));
                var value = Uri.UnescapeDataString(parts[1].Replace("+", " "));
                vnpayData[key] = value;
            }
        }

        return vnpayData;
    }

    private string HmacSHA512(string key, string inputData)
    {
        var hash = new StringBuilder();
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var inputBytes = Encoding.UTF8.GetBytes(inputData);
        
        using (var hmac = new HMACSHA512(keyBytes))
        {
            var hashValue = hmac.ComputeHash(inputBytes);
            foreach (var theByte in hashValue)
            {
                hash.Append(theByte.ToString("x2"));
            }
        }
        
        return hash.ToString();
    }

    // Helper method để loại bỏ dấu tiếng Việt
    private string RemoveVietnameseDiacritics(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(System.Text.NormalizationForm.FormC);
    }

    // Helper method để encode URL theo chuẩn VNPay
    // QUAN TRỌNG: VNPay yêu cầu thay %20 bằng + cho khoảng trắng
    // Theo hướng dẫn: encodeURIComponent(value).replace(/%20/g, "+")
    // Lưu ý: Không encode các ký tự đặc biệt như :, /, ?, &, = trong URL
    private string UrlEncodeForVnPay(string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        // Kiểm tra nếu là URL (có chứa ://) thì chỉ encode phần cần thiết
        if (value.Contains("://"))
        {
            // Đối với URL, encode toàn bộ nhưng giữ nguyên cấu trúc URL
            var encoded = Uri.EscapeDataString(value);
            // Thay %20 thành + cho khoảng trắng
            encoded = encoded.Replace("%20", "+");
            // Nhưng không encode các ký tự URL cơ bản - VNPay có thể yêu cầu encode cả URL
            return encoded;
        }

        // Đối với các giá trị khác, encode bình thường
        var encodedValue = Uri.EscapeDataString(value);
        // Thay %20 thành + cho khoảng trắng
        encodedValue = encodedValue.Replace("%20", "+");
        
        return encodedValue;
    }
    
    // Helper method để encode giá trị cho query string (dùng khi tính hash)
    // Khác với UrlEncodeForVnPay, method này chỉ encode các ký tự cần thiết
    private string EncodeValueForHash(string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        // Encode giá trị
        var encoded = Uri.EscapeDataString(value);
        // Thay %20 thành + (theo yêu cầu VNPay)
        encoded = encoded.Replace("%20", "+");
        
        return encoded;
    }
    
    // Helper method để chuyển IP address sang IPv4
    // QUAN TRỌNG: VNPay yêu cầu IP address phải là public IP, KHÔNG được dùng 127.0.0.1
    // - Nếu là localhost (127.0.0.1, ::1), cần thay bằng IP thực tế từ cấu hình
    // - Có thể lấy từ cấu hình VnPay:ClientIP
    private string GetIPv4Address(string ipAddress)
    {
        // Nếu là localhost hoặc IPv6 localhost, không được dùng
        if (string.IsNullOrEmpty(ipAddress) || 
            ipAddress == "127.0.0.1" || 
            ipAddress == "::1" || 
            ipAddress == "::ffff:127.0.0.1" ||
            ipAddress == "localhost")
        {
            return GetDefaultIP();
        }
        
        // Nếu đã là IPv4, kiểm tra xem có phải localhost không
        if (System.Net.IPAddress.TryParse(ipAddress, out var ip))
        {
            // Nếu là IPv6, lấy IPv4 mapped
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                // Lấy IPv4 từ IPv6 mapped
                if (ip.IsIPv4MappedToIPv6)
                {
                    var ipv4 = ip.MapToIPv4().ToString();
                    // Kiểm tra xem có phải localhost không
                    if (ipv4 == "127.0.0.1")
                        return GetDefaultIP();
                    return ipv4;
                }
                // Nếu là IPv6 thuần, sử dụng default IP
                return GetDefaultIP();
            }
            // Nếu đã là IPv4, kiểm tra xem có phải localhost không
            if (ipAddress == "127.0.0.1")
                return GetDefaultIP();
            return ipAddress;
        }
        
        // Nếu không parse được, trả về default IP
        return GetDefaultIP();
    }
    
    // Helper method để lấy default IP (không phải 127.0.0.1)
    // QUAN TRỌNG: VNPay yêu cầu public IP, không được dùng 127.0.0.1
    private string GetDefaultIP()
    {
        // Lấy từ cấu hình nếu có
        var configIP = _configuration["VnPay:ClientIP"];
        if (!string.IsNullOrEmpty(configIP) && 
            configIP != "127.0.0.1" && 
            configIP != "localhost" &&
            configIP != "::1")
        {
            return configIP;
        }
        
        // Nếu không có trong cấu hình, sử dụng IP mạng local
        // Trong môi trường test, có thể sử dụng IP này
        // Trong production, CẦN PHẢI có public IP thực tế
        return "192.168.1.1";
    }
}

