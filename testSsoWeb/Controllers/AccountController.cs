using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sustainsys.Saml2.AspNetCore2;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using testSsoWeb.ViewModels;

namespace testSsoWeb.Controllers
{
    public class AccountController : Controller
    {
        public async Task Login(string returnUrl)
        {
            var properties = new AuthenticationProperties();
            properties.RedirectUri = returnUrl ?? Url.Action("Claims", "Account");
            properties.Items["LoginProvider"] = Saml2Defaults.Scheme;
            await HttpContext.ChallengeAsync(Saml2Defaults.Scheme, properties);
        }

        [Authorize]
        public async Task Logout()
        {
            var properties = new AuthenticationProperties();
            // Indicate here where Auth0 should redirect the user after a logout.
            // Note that the resulting absolute Uri must be whitelisted in the
            // **Allowed Logout URLs** settings for the client.
            properties.RedirectUri = Url.Action("Index", "Home");
            properties.Items["LoginProvider"] = Saml2Defaults.Scheme;
            await HttpContext.SignOutAsync(Saml2Defaults.Scheme, properties);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// This is just a helper action to enable you to easily see all claims related to a user. It helps when debugging your application to see the in claims populated from the Auth0 ID Token
        /// </summary>
        [Authorize]
        public IActionResult Claims() => View();

        [Authorize]
        public IActionResult Profile()
        {
            return View(new UserProfileViewModel
            {
                Name = User.Identity.Name,
                NameIdentifier = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
                EmailAddress = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                ProfileImage = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.auth0.com/picture")?.Value,
            });
        }

        public IActionResult AccessDenied() => View();
    }
}
