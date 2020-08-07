using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace GGStream.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult SignIn()
        {
            var redirectUrl = Url.Action(nameof(HomeController.Index), "Home");
            return Challenge(
                new AuthenticationProperties {RedirectUri = redirectUrl},
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public IActionResult SignOut()
        {
            var callbackUrl = Url.Action(nameof(SignedOut), "Account", null, Request.Scheme);
            return SignOut(
                new AuthenticationProperties {RedirectUri = callbackUrl},
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public IActionResult SignedOut()
        {
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            ViewData["Message"] = "This stream isn't live right now.";
            ViewData["Color"] = "info";
            ViewData["Icon"] = "fad fa-calendar-times";

            return View("Error");
        }
    }
}