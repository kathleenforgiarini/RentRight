using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using RentRight.Data;
using RentRight.Models;
using System.Diagnostics;

namespace RentRight.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly RentRightContext _context; 
        private readonly ILogger<HomeController> _logger;

        public HomeController(RentRightContext context, ILogger<HomeController> logger)
        {
            _context = context; 
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"] as string;
               
                return View();
            }
            return RedirectToAction("Login", "Account");          
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
