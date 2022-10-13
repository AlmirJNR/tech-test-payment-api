using System.Security.Claims;

namespace ECommerce.Api.Utils;

public static class JwtUtil
{
    public static Guid GetSellerGuid(IEnumerable<Claim> claims)
    {
        var claim = claims.FirstOrDefault(c => c.Type == "sellerId")?.Value;
        Guid.TryParse(claim, out var sellerGuid);

        return sellerGuid;
    }
}