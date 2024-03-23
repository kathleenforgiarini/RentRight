using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RentRight.Controllers
{
    public class AccountController : Controller
    {
        // GET: AccountController
        public ActionResult Login()
        {
            return View();
        }
    }
}
