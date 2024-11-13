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
    public class TransactionController : ControllerBase
    {
        private readonly DATransaction transaction;

        public TransactionController(PlannerTrackerContext db)
        {
            transaction = new DATransaction(db);
        }

        [HttpPost]
        public async Task<ActionResult> Create(VMTransactionReq req)
        {
            try
            {
                VMResponse<VMTransaction> response = await Task.Run(() => transaction.Create(req));
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at TransactionController.Create: " + ex.Message);
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

        [HttpGet]
        public async Task<ActionResult> Fetch()
        {
            try
            {
                VMResponse<List<VMTransaction>> response = await Task.Run(() => transaction.Fetch());
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at TransactionController.Fetch: " + ex.Message);
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
