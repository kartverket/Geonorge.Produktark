using System;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Kartverket.Produktark.Helpers;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;

namespace Kartverket.Produktark.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "ProductSheets");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Route("setculture/{culture}")]
        public ActionResult SetCulture(string culture, string returnUrl)
        {
            // Validate input
            culture = CultureHelper.GetImplementedCulture(culture);
            // Save culture in a cookie
            HttpCookie cookie = Request.Cookies["_culture"];
            if (cookie != null)
                cookie.Value = culture;   // update cookie value
            else
            {
                cookie = new HttpCookie("_culture");
                cookie.Value = culture;
                cookie.Expires = DateTime.Now.AddYears(1);
            }
            Response.Cookies.Add(cookie);

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index");
        }

        public void SignIn()
        {
            var redirectUrl = Url.Action(nameof(HomeController.Index), "Home");
            HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = redirectUrl },
                OpenIdConnectAuthenticationDefaults.AuthenticationType);
        }

        public void SignOut()
        {
            var redirectUri = WebConfigurationManager.AppSettings["GeoID:PostLogoutRedirectUri"];
            HttpContext.GetOwinContext().Authentication.SignOut(
                new AuthenticationProperties { RedirectUri = redirectUri },
                OpenIdConnectAuthenticationDefaults.AuthenticationType,
                CookieAuthenticationDefaults.AuthenticationType);
        }

        /// <summary>
        /// This is the action responding to /signout-callback-oidc route after logout at the identity provider
        /// </summary>
        /// <returns></returns>
        public ActionResult SignOutCallback()
        {
            return RedirectToAction(nameof(ProductSheetsController.Index), "ProductSheets");
        }
    }
}