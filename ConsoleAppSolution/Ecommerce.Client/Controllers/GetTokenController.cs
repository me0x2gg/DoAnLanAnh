using Ecommerce.Client.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.Client.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class GetTokenController : Controller
    {

        private IIdentityServer4Service _identityServer4Service = null;
        public GetTokenController(IIdentityServer4Service identityServer4Service)
        {
            _identityServer4Service = identityServer4Service;
        }


        [HttpGet]
        public async Task<ActionResult> Token() // original method
        {
            string role = User.FindFirstValue("role");
            IdentityModel.Client.TokenResponse OAuth2Token = null;
            if (role == "Admin")
            {
                OAuth2Token = await _identityServer4Service.GetToken("customerApi.read customerApi.write");
            } else if (role == "Client")
            {
                OAuth2Token = await _identityServer4Service.GetToken("customerApi.read");
            } else
            {
                return Unauthorized("Only Admin or Client can have the token");
            }

            ViewData["role"] = role;
            ViewData["token"] = OAuth2Token.AccessToken;

            return View();
        }
    }
}
