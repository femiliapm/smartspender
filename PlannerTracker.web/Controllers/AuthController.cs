using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PlannerTracker.ViewModel;
using PlannerTracker.web.Models;
using System.Net;

namespace PlannerTracker.web.Controllers
{
    public class AuthController : Controller
    {
        private AuthModel auth;

        public AuthController(IConfiguration _config)
        {
            auth = new(_config);
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("auth") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("auth") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<VMResponse<VMAuth>?> Login(VMAuth vAuth)
        {
            VMResponse<VMAuth>? response = await auth.Login(vAuth);

            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                HttpContext.Session.SetString("auth", JsonConvert.SerializeObject(response.Data ?? new VMAuth()));
            }

            return response;
        }

        [HttpPost]
        public async Task<VMResponse<VMAuth>?> Register(VMAuth vAuth)
        {
            VMResponse<VMAuth>? response = await auth.Register(vAuth);
            return response;
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
