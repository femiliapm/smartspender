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

        [HttpGet("{id}")]
        public async Task<ActionResult> FetchById(Guid id)
        {
            try
            {
                VMResponse<VMBudgetPlan> response = await Task.Run(() => budgetPlan.GetById(id));
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

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(VMBudgetPlanReq req, Guid id)
        {
            try
            {
                VMResponse<VMBudgetPlan> response = await Task.Run(() => budgetPlan.Update(req, id));
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

        [HttpDelete("[action]")]
        public async Task<ActionResult> Delete(Guid id, Guid userId)
        {
            try
            {
                VMResponse<VMBudgetPlan> response = await Task.Run(() => budgetPlan.Delete(id, userId));
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

        [HttpDelete]
        public async Task<ActionResult> DeleteMultiple(string id, Guid userId)
        {
            try
            {
                List<string> idListStr = id.Split(",").ToList();
                List<Guid> ids = new();

                foreach (var idstr in idListStr)
                {
                    Guid temp = Guid.Parse(idstr);
                    ids.Add(temp);
                }

                VMResponse<VMBudgetPlan> response = await Task.Run(() => budgetPlan.DeleteMultiple(ids, userId));
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at BudgetPlanController.DeleteMultiple: " + ex.Message);
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
