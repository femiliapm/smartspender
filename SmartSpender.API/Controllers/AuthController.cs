using Microsoft.AspNetCore.Mvc;
using SmartSpender.DataAccess;
using SmartSpender.DataModel;
using SmartSpender.ViewModel;

namespace SmartSpender.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DAAuth auth;

        public AuthController(SmartSpenderContext db, IConfiguration config)
        {
            auth = new(db, config);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> Regist(VMAuth vAuth)
        {
            try
            {
                VMResponse<VMUser> response = await Task.Run(() => auth.RegisterRaw(vAuth));
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at AuthController.Regist: " + ex.Message);
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
        public async Task<ActionResult> Login(VMAuth vAuth)
        {
            try
            {
                VMResponse<VMAuth> response = await Task.Run(() => auth.LoginRaw(vAuth));
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at AuthController.Login: " + ex.Message);
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
