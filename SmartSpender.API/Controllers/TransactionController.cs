using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSpender.DataAccess;
using SmartSpender.DataModel;
using SmartSpender.ViewModel;

namespace SmartSpender.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly DATransaction transaction;

        public TransactionController(SmartSpenderContext db)
        {
            transaction = new(db);
        }

        [HttpPost]
        public async Task<ActionResult> Create(VMTransactionReq req)
        {
            try
            {
                VMResponse<VMTransaction> response = await Task.Run(() => transaction.CreateRaw(req));
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
        public async Task<ActionResult> Fetch(string? filter)
        {
            try
            {
                VMResponse<List<VMTransaction>> response = await Task.Run(() => transaction.FetchRaw(filter));
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

        [HttpGet("[action]")]
        public async Task<ActionResult> Category(string? by)
        {
            try
            {
                VMResponse<List<VMTransactionCategory>> response = await Task.Run(() => transaction.FetchExpenseByCategory());
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at TransactionController.Category: " + ex.Message);
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
