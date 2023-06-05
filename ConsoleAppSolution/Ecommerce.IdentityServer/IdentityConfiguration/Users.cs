using IdentityModel;
using IdentityServer4.Test;
using System.Security.Claims;

namespace Ecommerce.IdentityServer.IdentityConfiguration
{
    public class Users
    {
        public static List<TestUser> Get()
        {
            return new List<TestUser>
        {
            new TestUser
            {
                SubjectId = "56892347",
                Username = "admin",
                Password = "password",
                Claims = new List<Claim>
                {
                    new Claim(JwtClaimTypes.Email, "support@lananh.space"),
                    new Claim(JwtClaimTypes.Role, "Admin"),
                    new Claim(JwtClaimTypes.WebSite, "https://lananh.space")
                }
            },
            new TestUser
            {
                SubjectId = "56892348",
                Username = "client",
                Password = "password",
                Claims = new List<Claim>
                {
                    new Claim(JwtClaimTypes.Email, "support@lananh.space"),
                    new Claim(JwtClaimTypes.Role, "Client"),
                    new Claim(JwtClaimTypes.WebSite, "https://lananh.space")
                }
            },
            new TestUser
            {
                SubjectId = "56892349",
                Username = "user",
                Password = "password",
                Claims = new List<Claim>
                {
                    new Claim(JwtClaimTypes.Email, "support@lananh.space"),
                    new Claim(JwtClaimTypes.Role, "User"),
                    new Claim(JwtClaimTypes.WebSite, "https://lananh.space")
                }
            }
        };
        }
    }
}
