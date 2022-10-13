using System.Security.Claims;

namespace ECommerce.Api.Utils;

public static class JwtUtil
{
    public static Dictionary<string, string> ClaimsToDictionary(IEnumerable<Claim> claims)
        => claims.ToDictionary(c => c.Type, c => c.Value);

    public static Guid GetSellerGuid(Dictionary<string, string> claimsDictionary)
    {
        claimsDictionary.TryGetValue("sellerId", out var sellerId);
        Guid.TryParse(sellerId, out var sellerGuid);

        return sellerGuid;
    }
}