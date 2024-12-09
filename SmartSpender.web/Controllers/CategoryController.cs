using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartSpender.ViewModel;
using SmartSpender.web.AddOns;
using SmartSpender.web.Models;

namespace SmartSpender.web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly CategoryModel category;
        private readonly int pageSize;

        public CategoryController(IConfiguration _config)
        {
            category = new(_config);
            pageSize = int.Parse(_config["PageSize"]);
        }

        public async Task<IActionResult> Index(string? filter, int? currentPageSize, int pageNumber = 1)
        {
            string? authStr = HttpContext.Session.GetString("auth");
            VMAuth? auth = authStr != null ? JsonConvert.DeserializeObject<VMAuth?>(authStr) : null;
            if (auth == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["Title"] = "Manage Category";
            VMResponse<List<VMCategory>>? response = new();

            try
            {
                response = await category.Fetch(auth.Token ?? string.Empty, auth.Id);
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

        public IActionResult Create()
        {
            ViewBag.Title = "Add Category";
            return View();
        }

        [HttpPost]
        public async Task<VMResponse<VMCategory>?> AddCategory(VMCategoryReq req)
        {
            string? authStr = HttpContext.Session.GetString("auth");
            VMAuth? auth = authStr != null ? JsonConvert.DeserializeObject<VMAuth?>(authStr) : null;
            if (auth == null)
            {
                throw new Exception("Error not permission!");
            }
            req.ModifiedBy = auth.Id;

            VMResponse<VMCategory>? response = await category.Create(req, auth.Token!);

            return response;
        }
    }
}
