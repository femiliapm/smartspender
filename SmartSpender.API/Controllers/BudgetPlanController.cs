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
    public class BudgetPlanController : ControllerBase
    {
        private readonly DABudgetPlan budgetPlan;

        public BudgetPlanController(SmartSpenderContext db)
        {
            budgetPlan = new(db);
        }

        [HttpGet]
        public async Task<ActionResult> Fetch(string? filter)
        {
            try
            {
                VMResponse<List<VMBudgetPlan>> response = await Task.Run(() =>
                    budgetPlan.FetchAllRaw(filter));
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at BudgetPlanController.Fetch: " + ex.Message);
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

        [HttpGet("{id}")]
        public async Task<ActionResult> FetchById(Guid id)
        {
            try
            {
                VMResponse<VMBudgetPlan> response = await Task.Run(() => budgetPlan.FetchByIdRaw(id));
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at BudgetPlanController.FetchById: " + ex.Message);
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
        public async Task<ActionResult> Create(VMBudgetPlanReq req)
        {
            try
            {
                VMResponse<VMBudgetPlan> response = await Task.Run(() => budgetPlan.CreateRaw(req));
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at BudgetPlanController.Create: " + ex.Message);
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
        public async Task<ActionResult> Update(VMBudgetPlanReq req, Guid id)
        {
            try
            {
                VMResponse<VMBudgetPlan> response = await Task.Run(() => budgetPlan.UpdateRaw(req, id));
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at BudgetPlanController.Update: " + ex.Message);
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
                VMResponse<VMBudgetPlan> response = await Task.Run(() => budgetPlan.DeleteRaw(id));
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at BudgetPlanController.Delete: " + ex.Message);
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
