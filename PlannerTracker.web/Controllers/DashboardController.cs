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
        private ReminderModel reminder;

        public DashboardController(IConfiguration _config)
        {
            transaction = new(_config);
            budgetPlan = new(_config);
            reminder = new(_config);
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
            VMResponse<List<VMTransactionCategory>>? resExpense = await transaction.FetchByCategory(auth.Token ?? string.Empty);
            VMResponse<List<VMReminder>>? resReminder = await reminder.FetchAll(auth.Token ?? string.Empty, null);

            ViewData["Title"] = "Dashboard";
            ViewBag.Transactions = resTrans?.Data;
            ViewBag.Budgets = resBudget?.Data;
            ViewBag.ChartSpending = resExpense?.Data;
            ViewBag.Reminders = resReminder?.Data?.Where(r => r.IsCompleted == false).OrderBy(r => r.ReminderDate).ToList();

            return View();
        }
    }
}
