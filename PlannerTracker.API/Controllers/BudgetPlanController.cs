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
    public class BudgetPlanController : ControllerBase
    {
        private readonly DABudgetPlan budgetPlan;

        public BudgetPlanController(PlannerTrackerContext _db)
        {
            budgetPlan = new(_db);
        }

        [HttpGet]
        public async Task<ActionResult> Fetch()
        {
            try
            {
                VMResponse<List<VMBudgetPlan>> response = await Task.Run(() => budgetPlan.GetAll());
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

        [HttpPost]
        public async Task<ActionResult> Create(VMBudgetPlanReq req)
        {
            try
            {
                VMResponse<VMBudgetPlan> response = await Task.Run(() => budgetPlan.Create(req));
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
    }
}
