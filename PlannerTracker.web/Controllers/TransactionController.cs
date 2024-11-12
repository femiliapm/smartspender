using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PlannerTracker.ViewModel;
using PlannerTracker.web.Models;

namespace PlannerTracker.web.Controllers
{
    public class TransactionController : Controller
    {
        private CategoryModel category;
        private BudgetPlanModel budgetPlan;

        public TransactionController(IConfiguration _config)
        {
            category = new(_config);
            budgetPlan = new(_config);
        }

        public async Task<IActionResult> Create()
        {
            string? authStr = HttpContext.Session.GetString("auth");
            VMAuth? auth = authStr != null ? JsonConvert.DeserializeObject<VMAuth?>(authStr) : null;
            if (auth == null)
            {
                return RedirectToAction("Index", "Home");
            }

            VMResponse<List<VMCategory>>? resCategory = await category.Fetch(auth.Token ?? string.Empty, string.Empty);
            VMResponse<List<VMBudgetPlan>>? resBudgetPlan = await budgetPlan.Fetch(auth.Token ?? string.Empty, string.Empty);

            ViewBag.Title = "Add Transaction";
            ViewBag.Category = resCategory?.Data;
            ViewBag.BudgetPlan = resBudgetPlan?.Data;

            return View();
        }
    }
}
