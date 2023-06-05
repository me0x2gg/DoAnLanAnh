using IdentityModel.Client;

namespace Ecommerce.Client.Services
{
    public interface IIdentityServer4Service
    {
        Task<TokenResponse> GetToken(string apiScope);
    }
}
