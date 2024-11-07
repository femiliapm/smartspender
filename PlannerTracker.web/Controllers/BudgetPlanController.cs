using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PlannerTracker.ViewModel;
using PlannerTracker.web.Models;

namespace PlannerTracker.web.Controllers
{
    public class BudgetPlanController : Controller
    {
        private BudgetPlanModel budgetPlan;

        public BudgetPlanController(IConfiguration _config)
        {
            budgetPlan = new(_config);
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

            Console.WriteLine("data.TotalBudget: " + data?.TotalBudget);

            ViewBag.Title = "Edit Budget Plan";
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

            req.TotalBudget = Decimal.Parse(req.TotalBudgetStr.Replace('.', ','));
            req.TotalBudgetStr = string.Empty;

            return await budgetPlan.UpdateBudgetPlan(auth.Token ?? string.Empty, req, id);
        }

        public async Task<IActionResult> Settings()
        {
            string? authStr = HttpContext.Session.GetString("auth");
            VMAuth? auth = authStr != null ? JsonConvert.DeserializeObject<VMAuth?>(authStr) : null;
            if (auth == null)
            {
                return RedirectToAction("Index", "Home");
            }

            VMResponse<List<VMBudgetPlan>>? resBudgetPlan = await budgetPlan.Fetch(auth.Token ?? string.Empty);
            List<VMBudgetPlan>? data = resBudgetPlan?.Data ?? new();

            ViewData["Title"] = "Budget Plan Settings";
            return View(data);
        }
    }
}
