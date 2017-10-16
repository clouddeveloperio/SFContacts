using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFContacts.UI.Models;
using System.Security.Claims;

namespace SFContacts.UI.Controllers
{
    public class AccountController : Controller
    {
    
        [HttpGet]
        public IActionResult Login()
        {
            var cookies = new Dictionary<string, string>();
            foreach (var cookie in Request.Cookies) {
                cookies.Add(cookie.Key, cookie.Value);
            }
            ViewData["Cookies"] = cookies;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model) {
            if (LoginUser(model.Username, model.Password)) {
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, model.Username),
                    new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
                };
                var userIdentity = new ClaimsIdentity(claims, "login");
                ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                await HttpContext.Authentication.SignInAsync("CookieAuthentication", principal,
                    new Microsoft.AspNetCore.Http.Authentication.AuthenticationProperties { IsPersistent = true });

                return RedirectToAction("Index", "Profile");
            }
            return View();
        }

        private bool LoginUser(string username, string password) {
            return true;
        }

        [HttpGet]
        public async Task<IActionResult> Logout() {
            await HttpContext.Authentication.SignOutAsync("CookieAuthentication");
            foreach (var authCookieKey in HttpContext.Request.Cookies.Keys) {
                if (authCookieKey.IndexOf("CookieAuthentication") > -1) {
                    Response.Cookies.Delete(authCookieKey);
                }
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Unauthorized() {
            return View();
        }

    }
}