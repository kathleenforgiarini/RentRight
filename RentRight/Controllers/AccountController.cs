using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentRight.Data;
using RentRight.Models;
using System.Security.Claims;

namespace RentRight.Controllers
{
    public class AccountController : Controller
    {

        private readonly RentRightContext _context;

        public AccountController(RentRightContext context)
        {
            _context = context;
        }

        // GET: AccountController
        public IActionResult Login()
        {
            ViewBag.ShowLoginForm = true;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(String email, String password)
        {

            if (!ModelState.IsValid)
            {
                ViewBag.ShowLoginForm = true;
                return View();
            }
            var authenticatedUser = await _context.User.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

            if (authenticatedUser != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, authenticatedUser.Email),
                    new Claim("Type", authenticatedUser.Type),
                };

                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();

            }
        }

        [HttpPost]
        public IActionResult SignUp([Bind("FirstName,LastName,Email,Password")] User user, String? confirmPassword)
        {

            if (!ModelState.IsValid)
            {
                ViewBag.ShowLoginForm = false;
                return View("Login", user);
            }

            if (confirmPassword != user.Password)
            {
                ViewBag.ShowLoginForm = false;
                ModelState.AddModelError("", "Passwords do not match");
                return View("Login", user);

            }

            ViewBag.ShowLoginForm = true;
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
