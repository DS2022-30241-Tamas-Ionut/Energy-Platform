using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assignment1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminViewController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
