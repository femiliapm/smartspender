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
        private TagModel tag;
        private TransactionModel transaction;

        public TransactionController(IConfiguration _config)
        {
            category = new(_config);
            budgetPlan = new(_config);
            tag = new(_config);
            transaction = new(_config);
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
            VMResponse<List<string>>? resTag = await tag.FetchName(auth.Token ?? string.Empty);

            ViewBag.Title = "Add Transaction";
            ViewBag.Category = resCategory?.Data;
            ViewBag.BudgetPlan = resBudgetPlan?.Data;
            ViewBag.Tag = resTag?.Data;

            return View();
        }

        public async Task<IActionResult> Index()
        {
            string? authStr = HttpContext.Session.GetString("auth");
            VMAuth? auth = authStr != null ? JsonConvert.DeserializeObject<VMAuth?>(authStr) : null;
            if (auth == null)
            {
                return RedirectToAction("Index", "Home");
            }

            VMResponse<List<VMTransaction>>? resTrans = await transaction.FetchAll(auth.Token ?? string.Empty);

            ViewData["Title"] = "Transactions";
            ViewBag.Transactions = resTrans?.Data;

            return View();
        }

        [HttpPost]
        public async Task<VMResponse<VMTransaction>?> AddTransaction(VMTransactionReq req)
        {
            string? authStr = HttpContext.Session.GetString("auth");
            VMAuth? auth = authStr != null ? JsonConvert.DeserializeObject<VMAuth?>(authStr) : null;
            if (auth == null)
            {
                throw new Exception("Error not permission!");
            }

            req.ModifiedBy = auth.Id;

            return await transaction.SaveTransaction(req, auth.Token ?? string.Empty);
        }
    }
}
