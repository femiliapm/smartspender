﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartSpender.ViewModel;
using SmartSpender.web.Models;

namespace SmartSpender.web.Controllers
{
    public class BudgetPlanController : Controller
    {
        private readonly BudgetPlanModel budgetPlan;

        public BudgetPlanController(IConfiguration _config)
        {
            budgetPlan = new(_config);
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
    }
}
