using Microsoft.AspNetCore.Mvc;

namespace Assignment1.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
