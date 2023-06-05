using IdentityServer4.Models;

namespace Ecommerce.IdentityServer.IdentityConfiguration
{
    public class Scopes
    {
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new[]
            {
            new ApiScope("customerApi.read", "Read Access to Customer API"),
            new ApiScope("customerApi.write", "Write Access to Customer API"),
        };
        }
    }
}
