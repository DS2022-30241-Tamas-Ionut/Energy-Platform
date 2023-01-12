using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assignment1.Controllers
{
    public class ChatController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            var user = User.Identity.Name;

            return View("Index", user);
        }
    }
}
