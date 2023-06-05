using IdentityServer4.Models;

namespace Ecommerce.IdentityServer.IdentityConfiguration
{
    public class Resources
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new[]
            {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResource
            {
                Name = "role",
                UserClaims = new List<string> {"role"}
            }
        };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new[]
            {
            new ApiResource
            {
                Name = "customerApi",
                DisplayName = "Customer Api",
                Description = "Allow the application to access Customer Api on your behalf",
                Scopes = new List<string> { "customerApi.read", "customerApi.write"},
                ApiSecrets = new List<Secret> {new Secret("lananh".Sha256())},
                UserClaims = new List<string> {"role"}
            }
        };
        }
    }
}
