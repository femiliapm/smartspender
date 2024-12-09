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
    public class CategoryController : ControllerBase
    {
        private readonly DACategory category;

        public CategoryController(SmartSpenderContext db)
        {
            category = new(db);
        }

        [HttpGet]
        public async Task<ActionResult> FetchAll()
        {
            try
            {
                //VMResponse<List<VMCategory>> response = await Task.Run(() => category.FetchAllORM());
                VMResponse<List<VMCategory>> response = await Task.Run(() => category.FetchAllRaw(null));
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at CategoryController.FetchAll: " + ex.Message);
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

        [HttpGet("user/{userId}")]
        public async Task<ActionResult> FetchAll(Guid userId)
        {
            try
            {
                //VMResponse<List<VMCategory>> response = await Task.Run(() => category.FetchAllORM());
                VMResponse<List<VMCategory>> response = await Task.Run(() => category.FetchAllRaw(userId));
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at CategoryController.FetchAll: " + ex.Message);
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
                VMResponse<VMCategory> response = await Task.Run(() => category.FetchByIdRaw(id));
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

        [HttpPost]
        public async Task<ActionResult> Create(VMCategoryReq req)
        {
            try
            {
                //VMResponse<VMCategory> response = await Task.Run(() => category.CreateORM(req));
                VMResponse<VMCategory> response = await Task.Run(() => category.CreateRaw(req, null));
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

        [HttpPost("user/{userId}")]
        public async Task<ActionResult> Create(VMCategoryReq req, Guid userId)
        {
            try
            {
                //VMResponse<VMCategory> response = await Task.Run(() => category.CreateORM(req));
                VMResponse<VMCategory> response = await Task.Run(() => category.CreateRaw(req, userId));
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

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, VMCategoryReq req)
        {
            try
            {
                VMResponse<VMCategory> response = await Task.Run(() => category.UpdateRaw(req, id));
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
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                VMResponse<VMCategory> response = await Task.Run(() => category.DeleteRaw(id));
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
