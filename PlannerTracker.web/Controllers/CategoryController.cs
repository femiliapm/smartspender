using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PlannerTracker.ViewModel;
using PlannerTracker.web.Models;

namespace PlannerTracker.web.Controllers
{
    public class CategoryController : Controller
    {
        private CategoryModel category;

        public CategoryController(IConfiguration _config)
        {
            category = new(_config);
        }

        public IActionResult Create()
        {
            string? authStr = HttpContext.Session.GetString("auth");
            VMAuth? auth = authStr != null ? JsonConvert.DeserializeObject<VMAuth?>(authStr) : new();

            if ((auth != null && auth?.Role?.ToUpper() != "ADMIN") || auth == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Title = "Add Category";
            return View();
        }

        [HttpPost]
        public async Task<VMResponse<VMCategory>?> AddCategory(VMCategoryReq req)
        {
            string? authStr = HttpContext.Session.GetString("auth");
            VMAuth? auth = authStr != null ? JsonConvert.DeserializeObject<VMAuth?>(authStr) : new();
            if ((auth != null && auth?.Role?.ToUpper() != "ADMIN") || auth == null)
            {
                throw new Exception("Error not permission!");
            }

            req.CreatedBy = auth.Id;

            VMResponse<VMCategory>? response = await category.Create(req, auth.Token!);

            return response;
        }

        [HttpPost]
        public async Task<VMResponse<VMCategory>?> UpdateCategory(VMCategoryReq req, string id)
        {
            string? authStr = HttpContext.Session.GetString("auth");
            VMAuth? auth = authStr != null ? JsonConvert.DeserializeObject<VMAuth?>(authStr) : new();
            if ((auth != null && auth?.Role?.ToUpper() != "ADMIN") || auth == null)
            {
                throw new Exception("Error not permission!");
            }

            req.CreatedBy = auth.Id;

            VMResponse<VMCategory>? response = await category.Update(req, auth.Token!, id);

            return response;
        }

        [HttpPost]
        public async Task<VMResponse<VMCategory>?> DeleteCategory(string id)
        {
            string? authStr = HttpContext.Session.GetString("auth");
            VMAuth? auth = authStr != null ? JsonConvert.DeserializeObject<VMAuth?>(authStr) : new();
            if ((auth != null && auth?.Role?.ToUpper() != "ADMIN") || auth == null)
            {
                throw new Exception("Error not permission!");
            }

            VMResponse<VMCategory>? response = await category.Delete(auth.Id.ToString()!, auth.Token!, id);

            return response;
        }

        public async Task<IActionResult> Edit(string id)
        {
            string? authStr = HttpContext.Session.GetString("auth");
            VMAuth? auth = authStr != null ? JsonConvert.DeserializeObject<VMAuth?>(authStr) : new();

            if ((auth != null && auth?.Role?.ToUpper() != "ADMIN") || auth == null)
            {
                return RedirectToAction("Index", "Home");
            }

            VMResponse<VMCategory>? response = await category.FetchById(auth.Token!, id);
            VMCategory? data = response?.Data;

            ViewBag.Title = "Edit Category";
            return View(data);
        }

        public async Task<IActionResult> Delete(string id)
        {
            string? authStr = HttpContext.Session.GetString("auth");
            VMAuth? auth = authStr != null ? JsonConvert.DeserializeObject<VMAuth?>(authStr) : new();

            if ((auth != null && auth?.Role?.ToUpper() != "ADMIN") || auth == null)
            {
                return RedirectToAction("Index", "Home");
            }

            VMResponse<VMCategory>? response = await category.FetchById(auth.Token!, id);
            VMCategory? data = response?.Data;

            ViewBag.Title = "Delete Category";
            return View(data);
        }
    }
}
