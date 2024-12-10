using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartSpender.ViewModel;
using SmartSpender.web.Models;

namespace SmartSpender.web.Controllers
{
    public class TransactionController : Controller
    {
        private readonly TransactionModel transaction;
        private readonly CategoryModel category;
        private readonly BudgetPlanModel budgetPlan;
        private readonly TagModel tag;

        public TransactionController(IConfiguration _config)
        {
            transaction = new(_config);
            category = new(_config);
            budgetPlan = new(_config);
            tag = new(_config);
        }

        public IActionResult Index()
        {
            string? authStr = HttpContext.Session.GetString("auth");
            VMAuth? auth = authStr != null ? JsonConvert.DeserializeObject<VMAuth?>(authStr) : null;
            if (auth == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public async Task<IActionResult> Create()
        {
            string? authStr = HttpContext.Session.GetString("auth");
            VMAuth? auth = authStr != null ? JsonConvert.DeserializeObject<VMAuth?>(authStr) : null;
            if (auth == null)
            {
                return RedirectToAction("Index", "Home");
            }

            VMResponse<List<VMCategory>>? resCategory = await category.Fetch(auth.Token ?? string.Empty, auth.Id);
            VMResponse<List<VMBudgetPlan>>? resBudgetPlan = await budgetPlan.Fetch(auth.Token ?? string.Empty, string.Empty, auth.Id);
            VMResponse<List<string>>? resTag = await tag.FetchName(auth.Token ?? string.Empty);

            ViewBag.Title = "Add Transaction";
            ViewBag.Category = resCategory?.Data?.OrderBy(c => c.CategoryName).ToList();
            ViewBag.BudgetPlan = resBudgetPlan?.Data?.OrderBy(bp => bp.PlanName).ToList();
            ViewBag.Tag = resTag?.Data?.OrderBy(t => t).ToList();

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
