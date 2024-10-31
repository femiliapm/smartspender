using Microsoft.AspNetCore.Mvc;

namespace PlannerTracker.web.Controllers
{
    public class BudgetPlanController : Controller
    {
        public IActionResult Create()
        {
            ViewBag.Title = "Add Budget Plan";
            return View();
        }
    }
}
