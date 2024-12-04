using Microsoft.AspNetCore.Mvc;

namespace SmartSpender.web.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
