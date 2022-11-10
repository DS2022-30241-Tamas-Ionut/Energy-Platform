using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assignment1.Controllers
{
    [Authorize(Roles = "User")]
    public class UserView : Controller
    {
        public IActionResult Index()
        {
            var user = User.Identity.Name;

            return View("Index", user);
        }
    }
}
