using System.Security.Cryptography;
using System.Text;

namespace Api.Extensions
{
    public static class HttpContextExtensions
    {
        public static string? GetHashedIp(this HttpContext httpContext, string salt)
        {
            var ipAddress = httpContext.GetClientIpAddress();
            if (string.IsNullOrEmpty(ipAddress))
                return null;

            using var sha256 = SHA256.Create();
            var combined = ipAddress + salt;
            var bytes = Encoding.UTF8.GetBytes(combined);
            var hash = sha256.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }

        public static string? GetClientIpAddress(this HttpContext httpContext)
        {
            var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }

            return httpContext.Connection.RemoteIpAddress?.ToString();
        }
    }
}
