using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PlannerTracker.ViewModel;

namespace PlannerTracker.web.Controllers
{
    public class ReminderController : Controller
    {
        public IActionResult Index()
        {
            string? authStr = HttpContext.Session.GetString("auth");
            VMAuth? auth = authStr != null ? JsonConvert.DeserializeObject<VMAuth?>(authStr) : null;
            if (auth == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["Title"] = "Reminders";

            return View();
        }

        public IActionResult Create()
        {
            string? authStr = HttpContext.Session.GetString("auth");
            VMAuth? auth = authStr != null ? JsonConvert.DeserializeObject<VMAuth?>(authStr) : null;
            if (auth == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Title = "Add Reminder";

            return View();
        }
    }
}
