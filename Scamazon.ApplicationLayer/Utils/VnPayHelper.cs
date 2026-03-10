using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace MV.ApplicationLayer.Utils;

public static class VnPayHelper
{
    public static string HmacSHA512(string key, string inputData)
    {
        var hash = new StringBuilder();
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var inputBytes = Encoding.UTF8.GetBytes(inputData);
        using var hmac = new HMACSHA512(keyBytes);
        var hashValue = hmac.ComputeHash(inputBytes);
        foreach (var b in hashValue)
        {
            hash.Append(b.ToString("x2"));
        }
        return hash.ToString();
    }

    public static string CreatePaymentUrl(SortedList<string, string> vnpParams, string vnpUrl, string hashSecret)
    {
        var hashData = new StringBuilder();
        var urlQuery = new StringBuilder();

        foreach (var kv in vnpParams)
        {
            if (!string.IsNullOrEmpty(kv.Value))
            {
                if (hashData.Length > 0)
                {
                    hashData.Append('&');
                    urlQuery.Append('&');
                }
                // Hash và URL: encode cả key và value theo chuẩn VNPay v2.1.0
                hashData.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value));
                urlQuery.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value));
            }
        }

        var secureHash = HmacSHA512(hashSecret, hashData.ToString());
        return $"{vnpUrl}?{urlQuery}&vnp_SecureHash={secureHash}";
    }

    /// <summary>
    /// Xác thực chữ ký HMAC-SHA512 từ VNPay IPN hoặc Return URL
    /// </summary>
    public static bool ValidateSignature(IQueryCollection queryParams, string hashSecret)
    {
        var secureHash = queryParams["vnp_SecureHash"].ToString();
        if (string.IsNullOrEmpty(secureHash)) return false;

        var sortedParams = new SortedList<string, string>();
        foreach (var kv in queryParams)
        {
            if (kv.Key != "vnp_SecureHash" && kv.Key != "vnp_SecureHashType")
            {
                sortedParams.Add(kv.Key, kv.Value.ToString());
            }
        }

        // Build query string với URL-encode (khớp với cách VNPay tính hash lúc gửi về)
        var queryBuilder = new StringBuilder();
        foreach (var kv in sortedParams)
        {
            if (!string.IsNullOrEmpty(kv.Value))
            {
                if (queryBuilder.Length > 0) queryBuilder.Append('&');
                queryBuilder.Append(WebUtility.UrlEncode(kv.Key));
                queryBuilder.Append('=');
                queryBuilder.Append(WebUtility.UrlEncode(kv.Value));
            }
        }

        var computedHash = HmacSHA512(hashSecret, queryBuilder.ToString());
        return string.Equals(computedHash, secureHash, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Xác thực chữ ký từ Dictionary (dùng cho IPN)
    /// </summary>
    public static bool ValidateSignature(Dictionary<string, string> queryParams, string hashSecret)
    {
        if (!queryParams.TryGetValue("vnp_SecureHash", out var secureHash) || string.IsNullOrEmpty(secureHash))
            return false;

        var sortedParams = new SortedList<string, string>();
        foreach (var kv in queryParams)
        {
            if (kv.Key != "vnp_SecureHash" && kv.Key != "vnp_SecureHashType")
            {
                sortedParams.Add(kv.Key, kv.Value);
            }
        }

        // Build query string với URL-encode (khớp với cách VNPay tính hash lúc gửi về)
        var queryBuilder = new StringBuilder();
        foreach (var kv in sortedParams)
        {
            if (!string.IsNullOrEmpty(kv.Value))
            {
                if (queryBuilder.Length > 0) queryBuilder.Append('&');
                queryBuilder.Append(WebUtility.UrlEncode(kv.Key));
                queryBuilder.Append('=');
                queryBuilder.Append(WebUtility.UrlEncode(kv.Value));
            }
        }

        var computedHash = HmacSHA512(hashSecret, queryBuilder.ToString());
        return string.Equals(computedHash, secureHash, StringComparison.OrdinalIgnoreCase);
    }
}
