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
    public class CategoryController : ControllerBase
    {
        private readonly DACategory category;

        public CategoryController(PlannerTrackerContext db)
        {
            category = new DACategory(db);
        }

        [HttpPost]
        public async Task<ActionResult> Create(VMCategoryReq req)
        {
            try
            {
                VMResponse<VMCategory> response = await Task.Run(() => category.Create(req));
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at CategoryController.Create: " + ex.Message);
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
        public async Task<ActionResult> GetAll(string? filter)
        {
            try
            {
                VMResponse<List<VMCategory>> response = await Task.Run(() => category.GetCategories(filter ?? string.Empty));
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at CategoryController.GetAll: " + ex.Message);
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
        public async Task<ActionResult> GetById(Guid id)
        {
            try
            {
                VMResponse<VMCategory> response = await Task.Run(() => category.GetCategoryById(id));
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at CategoryController.GetById: " + ex.Message);
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
        public async Task<ActionResult> Update(Guid id, VMCategoryReq req)
        {
            try
            {
                VMResponse<VMCategory> response = await Task.Run(() => category.Update(req, id));
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at CategoryController.Update: " + ex.Message);
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
        public async Task<ActionResult> Delete(Guid id, Guid userId)
        {
            try
            {
                VMResponse<VMCategory> response = await Task.Run(() => category.DeleteById(id, userId));
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at CategoryController.Delete: " + ex.Message);
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
