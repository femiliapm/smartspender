using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PlannerTracker.ViewModel;
using PlannerTracker.web.AddOns;
using PlannerTracker.web.Models;

namespace PlannerTracker.web.Controllers
{
    public class ReminderController : Controller
    {
        private ReminderModel reminder;

        private readonly int pageSize;

        public ReminderController(IConfiguration _config)
        {
            reminder = new(_config);
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

            Console.WriteLine(filter + " filter reminder");
            Console.WriteLine(currentPageSize + " currentPageSize");
            Console.WriteLine(pageNumber + " pageNumber");

            VMReminderFilter? filterObj = JsonConvert.DeserializeObject<VMReminderFilter>(filter ?? string.Empty);

            VMResponse<List<VMReminder>>? resReminder = await reminder.FetchAll(auth.Token ?? string.Empty, filterObj);
            List<VMReminder> data = resReminder?.Data ?? new();
            data = data.OrderByDescending(r => r.ReminderDate).ToList();

            ViewData["Title"] = "Reminders";
            ViewBag.Status = filterObj?.IsCompleted;
            ViewBag.DateFrom = filterObj?.DateFrom;
            ViewBag.DateTo = filterObj?.DateTo;
            ViewBag.PageSize = currentPageSize ?? pageSize;
            ViewBag.FirstIdx = ((pageNumber - 1) * ViewBag.PageSize) + 1;
            ViewBag.LastIdx = Math.Min(data.Count, ViewBag.PageSize * pageNumber);

            return View(Pagination<VMReminder>.Create(data ?? new(), pageNumber, ViewBag.PageSize));
        }

        public IActionResult Create()
        {
            string? authStr = HttpContext.Session.GetString("auth");
            VMAuth? auth = authStr != null ? JsonConvert.DeserializeObject<VMAuth?>(authStr) : null;
            if (auth == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Title = "Add Reminder";

            return View();
        }

        [HttpPost]
        public async Task<VMResponse<VMReminder>?> AddReminder(VMReminderReq req)
        {
            string? authStr = HttpContext.Session.GetString("auth");
            VMAuth? auth = authStr != null ? JsonConvert.DeserializeObject<VMAuth?>(authStr) : null;
            if (auth == null)
            {
                throw new Exception("Error not permission!");
            }

            req.ModifiedBy = auth.Id;
            req.UserId = auth.Id ?? default;

            return await reminder.SaveReminder(req, auth.Token ?? string.Empty);
        }
    }
}
