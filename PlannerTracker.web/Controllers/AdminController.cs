using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PlannerTracker.ViewModel;
using PlannerTracker.web.AddOns;
using PlannerTracker.web.Models;

namespace PlannerTracker.web.Controllers
{
    public class AdminController : Controller
    {
        private CategoryModel category;
        private readonly int pageSize;

        public AdminController(IConfiguration _config)
        {
            category = new(_config);
            pageSize = int.Parse(_config["PageSize"]);
        }

        public IActionResult Index()
        {
            string? authStr = HttpContext.Session.GetString("auth");
            VMAuth? auth = authStr != null ? JsonConvert.DeserializeObject<VMAuth?>(authStr) : null;
            if ((auth != null && auth?.Role?.ToUpper() != "ADMIN") || auth == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public async Task<IActionResult> Category(string? filter, int? currentPageSize, int pageNumber = 1)
        {
            string? authStr = HttpContext.Session.GetString("auth");
            VMAuth? auth = authStr != null ? JsonConvert.DeserializeObject<VMAuth?>(authStr) : null;
            if ((auth != null && auth?.Role?.ToUpper() != "ADMIN") || auth == null)
            {
                return RedirectToAction("Index", "Home");
            }

            VMResponse<List<VMCategory>>? response = new();

            try
            {
                response = await category.Fetch(auth.Token ?? string.Empty, filter ?? string.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                VMError? err = JsonConvert.DeserializeObject<VMError>(ex.Message);
                if (err != null && !string.IsNullOrEmpty(err.error)) ViewBag.Error = err.error;
                if (err != null && !string.IsNullOrEmpty(err.error_description)) ViewBag.ErrorDesc = err.error_description;
            }

            List<VMCategory> data = response != null ? response.Data ?? new() : new();

            ViewBag.Filter = filter;
            ViewBag.PageSize = currentPageSize ?? pageSize;
            ViewBag.FirstIdx = ((pageNumber - 1) * ViewBag.PageSize) + 1;
            ViewBag.LastIdx = Math.Min(data.Count, ViewBag.PageSize * pageNumber);

            return View(Pagination<VMCategory>.Create(data ?? new(), pageNumber, ViewBag.PageSize));
        }
    }
}
