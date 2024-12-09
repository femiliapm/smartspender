using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartSpender.ViewModel;
using SmartSpender.web.Models;

namespace SmartSpender.web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly CategoryModel category;

        public CategoryController(IConfiguration _config)
        {
            category = new(_config);
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Manage Category";
            return View();
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
