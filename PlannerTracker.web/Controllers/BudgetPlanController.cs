using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PlannerTracker.ViewModel;
using PlannerTracker.web.AddOns;
using PlannerTracker.web.Models;

namespace PlannerTracker.web.Controllers
{
    public class BudgetPlanController : Controller
    {
        private BudgetPlanModel budgetPlan;
        private readonly int pageSize;

        public BudgetPlanController(IConfiguration _config)
        {
            budgetPlan = new(_config);
            pageSize = int.Parse(_config["PageSize"]);
        }

        public IActionResult Create()
        {
            string? authStr = HttpContext.Session.GetString("auth");
            VMAuth? auth = authStr != null ? JsonConvert.DeserializeObject<VMAuth?>(authStr) : null;
            if (auth == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Title = "Add Budget Plan";
            return View();
        }

        public async Task<IActionResult> Edit(string id)
        {
            string? authStr = HttpContext.Session.GetString("auth");
            VMAuth? auth = authStr != null ? JsonConvert.DeserializeObject<VMAuth?>(authStr) : null;
            if (auth == null)
            {
                return RedirectToAction("Index", "Home");
            }

            VMResponse<VMBudgetPlan>? response = await budgetPlan.FetchById(auth.Token ?? string.Empty, id);
            VMBudgetPlan? data = response?.Data ?? null;

            ViewBag.Title = "Edit Budget Plan";
            return View(data);
        }

        public async Task<IActionResult> Delete(string id)
        {
            string? authStr = HttpContext.Session.GetString("auth");
            VMAuth? auth = authStr != null ? JsonConvert.DeserializeObject<VMAuth?>(authStr) : null;
            if (auth == null)
            {
                return RedirectToAction("Index", "Home");
            }

            List<string> idstr = id.Split(",").ToList();
            List<VMBudgetPlan> data = new();

            foreach (var item in idstr)
            {
                VMResponse<VMBudgetPlan>? response = await budgetPlan.FetchById(auth.Token ?? string.Empty, item);
                VMBudgetPlan temp = response?.Data!;
                data.Add(temp);
            }

            ViewBag.Title = "Delete Budget Plan";
            return View(data);
        }

        [HttpPost]
        public async Task<VMResponse<VMBudgetPlan>?> AddBudgetPlan(VMBudgetPlanReq req)
        {
            string? authStr = HttpContext.Session.GetString("auth");
            VMAuth? auth = authStr != null ? JsonConvert.DeserializeObject<VMAuth?>(authStr) : null;
            if (auth == null)
            {
                throw new Exception("Error not permission!");
            }

            req.ModifiedBy = auth.Id;
            req.UserId = auth.Id ?? default;

            return await budgetPlan.SaveBudgetPlan(auth.Token ?? string.Empty, req);
        }

        [HttpPost]
        public async Task<VMResponse<VMBudgetPlan>?> EditBudgetPlan(VMBudgetPlanReq req, string id)
        {
            string? authStr = HttpContext.Session.GetString("auth");
            VMAuth? auth = authStr != null ? JsonConvert.DeserializeObject<VMAuth?>(authStr) : null;
            if (auth == null)
            {
                throw new Exception("Error not permission!");
            }

            req.ModifiedBy = auth.Id ?? default;
            req.UserId = auth.Id ?? default;

            req.TotalBudget = Decimal.Parse(req.TotalBudgetStr?.Replace('.', ',') ?? "0");
            req.TotalBudgetStr = string.Empty;

            return await budgetPlan.UpdateBudgetPlan(auth.Token ?? string.Empty, req, id);
        }

        [HttpPost]
        public async Task<VMResponse<VMBudgetPlan>?> DeleteBudgetPlan(string id)
        {
            string? authStr = HttpContext.Session.GetString("auth");
            VMAuth? auth = authStr != null ? JsonConvert.DeserializeObject<VMAuth?>(authStr) : null;
            if (auth == null)
            {
                throw new Exception("Error not permission!");
            }

            return await budgetPlan.DeleteBudgetPlan(auth.Token ?? string.Empty, id, auth.Id.ToString() ?? string.Empty);
        }

        public async Task<IActionResult> Settings(string? filter, int? currentPageSize, int pageNumber = 1)
        {
            string? authStr = HttpContext.Session.GetString("auth");
            VMAuth? auth = authStr != null ? JsonConvert.DeserializeObject<VMAuth?>(authStr) : null;
            if (auth == null)
            {
                return RedirectToAction("Index", "Home");
            }

            VMResponse<List<VMBudgetPlan>>? resBudgetPlan = await budgetPlan.Fetch(auth.Token ?? string.Empty, filter);
            List<VMBudgetPlan>? data = resBudgetPlan?.Data ?? new();

            ViewData["Title"] = "Budget Plan Settings";
            ViewBag.Filter = filter;
            ViewBag.PageSize = currentPageSize ?? pageSize;
            ViewBag.FirstIdx = ((pageNumber - 1) * ViewBag.PageSize) + 1;
            ViewBag.LastIdx = Math.Min(data.Count, ViewBag.PageSize * pageNumber);

            return View(Pagination<VMBudgetPlan>.Create(data ?? new(), pageNumber, ViewBag.PageSize));
        }

        public async Task<IActionResult> Index(int? currentPageSize, int pageNumber = 1)
        {
            string? authStr = HttpContext.Session.GetString("auth");
            VMAuth? auth = authStr != null ? JsonConvert.DeserializeObject<VMAuth?>(authStr) : null;
            if (auth == null)
            {
                return RedirectToAction("Index", "Home");
            }

            VMResponse<List<VMBudgetPlan>>? resBudget = await budgetPlan.Fetch(auth.Token ?? string.Empty, string.Empty);
            List<VMBudgetPlan>? data = resBudget?.Data ?? new();

            ViewData["Title"] = "Budget Plan";
            ViewBag.Budgets = resBudget?.Data;
            ViewBag.PageSize = currentPageSize ?? pageSize;
            ViewBag.FirstIdx = ((pageNumber - 1) * ViewBag.PageSize) + 1;
            ViewBag.LastIdx = Math.Min(data.Count, ViewBag.PageSize * pageNumber);

            return View(Pagination<VMBudgetPlan>.Create(data ?? new(), pageNumber, ViewBag.PageSize));
        }
    }
}
