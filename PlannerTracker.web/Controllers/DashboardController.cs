using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PlannerTracker.ViewModel;
using PlannerTracker.web.Models;

namespace PlannerTracker.web.Controllers
{
    public class DashboardController : Controller
    {
        private TransactionModel transaction;
        private BudgetPlanModel budgetPlan;

        public DashboardController(IConfiguration _config)
        {
            transaction = new(_config);
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

            VMResponse<List<VMTransaction>>? resTrans = await transaction.FetchAll(auth.Token ?? string.Empty, string.Empty);
            VMResponse<List<VMBudgetPlan>>? resBudget = await budgetPlan.Fetch(auth.Token ?? string.Empty, string.Empty);

            ViewData["Title"] = "Dashboard";
            ViewBag.Transactions = resTrans?.Data;
            ViewBag.Budgets = resBudget?.Data;

            return View();
        }
    }
}
