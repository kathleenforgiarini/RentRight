using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentRight.Data;
using RentRight.Models;
using System.Security.Claims;
using RentRight.Utilities;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.AspNetCore.Authorization;

namespace RentRight.Controllers
{
    public class AccountController : Controller
    {

        private readonly RentRightContext _context;
        private readonly AuthService _authService;

        public AccountController(RentRightContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        // GET: AccountController
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
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

            var isAuthenticated = await _authService.AuthenticateUserAsync(email, password, HttpContext);

            if (isAuthenticated)
            {
                TempData["SuccessMessage"] = "Welcome!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid Credentials";
                ViewBag.ShowLoginForm = true;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> SignUp([Bind("FirstName,LastName,Email,Password")] User user, String? confirmPassword)
        {

            if (!ModelState.IsValid)
            {
                ViewBag.ShowLoginForm = false;
                return View("Login");
            }

            if (confirmPassword != user.Password)
            {
                ViewBag.ShowLoginForm = false;
                ViewBag.ErrorMessage = "Passwords do not match!";
                return View("Login");
            }

            var existingUser = await _context.User.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser != null)
            {
                ViewBag.ShowLoginForm = false;
                ViewBag.ErrorMessage = "This e-mail already exists.";
                return View("Login");
            }

            try
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                var isAuthenticated = await _authService.AuthenticateUserAsync(user.Email, user.Password, HttpContext);

                if (isAuthenticated)
                {
                    TempData["SuccessMessage"] = "Welcome!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.ErrorMessage = "An error occured. Try again!";
                    ViewBag.ShowLoginForm = true;
                    return View("Login");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Login");
            }
        }

        [Authorize]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Index", "Home");
        }

    }
}
