using IdentityServer4.Models;
using IdentityServer4;

namespace Ecommerce.IdentityServer.IdentityConfiguration
{
    public class Clients
    {
        public static IEnumerable<Client> Get()
        {
            return new List<Client>
            {       
                new Client
                {
                    ClientId = "customerApi",
                    ClientName = "ASP.NET Core Customer Api",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = new List<Secret> {new Secret("benasin".Sha256())},
                    AllowedScopes = new List<string> {"customerApi.read", "customerApi.write" }
                },
                new Client
                {
                    ClientId = "oidcMVCApp",
                    ClientName = "Sample ASP.NET Core MVC Web App",
                    ClientSecrets = new List<Secret> {new Secret("benasin".Sha256())},

                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = new List<string> {"https://localhost:7122/signin-oidc"},
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "role",
                        "weatherApi.read",
                    },
                    RequirePkce = true,
                    AllowPlainTextPkce = false
                }
            };
        }
    }
}
