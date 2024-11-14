using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PlannerTracker.ViewModel;
using PlannerTracker.web.AddOns;
using PlannerTracker.web.Models;

namespace PlannerTracker.web.Controllers
{
    public class TransactionController : Controller
    {
        private CategoryModel category;
        private BudgetPlanModel budgetPlan;
        private TagModel tag;
        private TransactionModel transaction;

        private readonly int pageSize;

        public TransactionController(IConfiguration _config)
        {
            category = new(_config);
            budgetPlan = new(_config);
            tag = new(_config);
            transaction = new(_config);
            pageSize = int.Parse(_config["PageSize"]);
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
            ViewBag.Category = resCategory?.Data?.OrderBy(c => c.CategoryName).ToList();
            ViewBag.BudgetPlan = resBudgetPlan?.Data?.OrderBy(bp => bp.PlanName).ToList();
            ViewBag.Tag = resTag?.Data?.OrderBy(t => t).ToList();

            return View();
        }

        public async Task<IActionResult> Index(string? filter, int? currentPageSize, int pageNumber = 1)
        {
            string? authStr = HttpContext.Session.GetString("auth");
            VMAuth? auth = authStr != null ? JsonConvert.DeserializeObject<VMAuth?>(authStr) : null;
            if (auth == null)
            {
                return RedirectToAction("Index", "Home");
            }

            VMResponse<List<VMTransaction>>? resTrans = await transaction.FetchAll(auth.Token ?? string.Empty, filter);
            List<VMTransaction> data = resTrans?.Data ?? new();

            ViewData["Title"] = "Transactions";
            ViewBag.Filter = filter;
            ViewBag.PageSize = currentPageSize ?? pageSize;
            ViewBag.FirstIdx = ((pageNumber - 1) * ViewBag.PageSize) + 1;
            ViewBag.LastIdx = Math.Min(data.Count, ViewBag.PageSize * pageNumber);

            return View(Pagination<VMTransaction>.Create(data ?? new(), pageNumber, ViewBag.PageSize));
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
