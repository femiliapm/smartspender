using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlannerTracker.DataAccess;
using PlannerTracker.DataModel;
using PlannerTracker.ViewModel;

namespace PlannerTracker.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NativeController : ControllerBase
    {
        private readonly DANative native;

        public NativeController(PlannerTrackerContext db)
        {
            native = new(db);
        }

        [HttpGet]
        public async Task<ActionResult> Fetch()
        {
            try
            {
                VMResponse<List<Reminder>> response = await Task.Run(() => native.FetchAll());
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at NativeController.Fetch: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("InnerException: " + ex.InnerException.Message);
                }

                VMError err = new()
                {
                    error = ex.Message,
                    error_description = ex.Source
                };

                return StatusCode(500, err);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create(VMReminderReq req)
        {
            try
            {
                VMResponse<int> response = await Task.Run(() => native.Create(req));
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at NativeController.Create: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("InnerException: " + ex.InnerException.Message);
                }

                VMError err = new()
                {
                    error = ex.Message,
                    error_description = ex.Source
                };

                return StatusCode(500, err);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, VMReminderReq req)
        {
            try
            {
                VMResponse<int> response = await Task.Run(() => native.Update(id, req));
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at NativeController.Update: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("InnerException: " + ex.InnerException.Message);
                }

                VMError err = new()
                {
                    error = ex.Message,
                    error_description = ex.Source
                };

                return StatusCode(500, err);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                VMResponse<int> response = await Task.Run(() => native.Delete(id));
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at NativeController.Delete: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("InnerException: " + ex.InnerException.Message);
                }

                VMError err = new()
                {
                    error = ex.Message,
                    error_description = ex.Source
                };

                return StatusCode(500, err);
            }
        }
    }
}
