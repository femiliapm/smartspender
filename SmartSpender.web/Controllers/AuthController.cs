using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartSpender.ViewModel;
using SmartSpender.web.Models;
using System.Net;

namespace SmartSpender.web.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthModel auth;

        public AuthController(IConfiguration _config)
        {
            auth = new(_config);
        }

        public IActionResult Index()
        {
            string? authStr = HttpContext.Session.GetString("auth");
            VMAuth? auth = authStr != null ? JsonConvert.DeserializeObject<VMAuth?>(authStr) : null;
            if (auth != null && auth?.Token != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<VMResponse<VMAuth>?> Login(VMAuth req)
        {
            VMResponse<VMAuth>? response = await auth.Login(req);

            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                HttpContext.Session.SetString("auth", JsonConvert.SerializeObject(response.Data ?? new VMAuth()));
            }

            return response;
        }

        [HttpPost]
        public async Task<VMResponse<VMAuth>?> Register(VMAuth req)
        {
            VMResponse<VMAuth>? response = await auth.Register(req);
            return response;
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
