using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartSpender.ViewModel;
using SmartSpender.web.Models;

namespace SmartSpender.web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly BudgetPlanModel budgetPlan;

        public DashboardController(IConfiguration _config)
        {
            budgetPlan = new(_config);
        }

        public async Task<IActionResult> Index()
        {
            string? authStr = HttpContext.Session.GetString("auth");
            VMAuth? auth = authStr != null ? JsonConvert.DeserializeObject<VMAuth?>(authStr) : null;
            if (auth == null)
            {
                return RedirectToAction("Index", "Home");
            }

            VMResponse<List<VMBudgetPlan>>? resBudget = await budgetPlan.Fetch(auth.Token ?? string.Empty, string.Empty, auth.Id);

            ViewBag.Budgets = resBudget?.Data;

            return View();
        }
    }
}
