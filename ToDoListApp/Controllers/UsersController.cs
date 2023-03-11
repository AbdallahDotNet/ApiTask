using Interfaces.Helper.ResponseApi;
using Interfaces.Helpers;
using Interfaces.Interfaces;
using Interfaces.ViewModels.UserVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace ToDoListApp.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public static NLog.ILogger logger = LogManager.GetCurrentClassLogger();
        private IUser _repo;
        public UsersController(IUser repo)
        {
            _repo = repo;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                return Ok(await _repo.Login(model));
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError(ex, logger, "[/api/v1/users/login]");

                return BadRequest(new RequestResponse
                {
                    RequestState = "FAIL",
                    RequestMessage = "حدث خطأ برجاء المحاوله لاحقا",
                    Response = null
                });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create(SaveUserViewModel model)
        {
            try
            {
                return Ok(await _repo.CreateUser(model));
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError(ex, logger, "[/api/v1/users/create]");

                return BadRequest(new RequestResponse
                {
                    RequestState = "FAIL",
                    RequestMessage = "حدث خطأ برجاء المحاوله لاحقا",
                    Response = null
                });
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(SaveUserViewModel model)
        {
            try
            {
                return Ok(await _repo.UpdateUser(model));
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError(ex, logger, "[/api/v1/users/update]");

                return BadRequest(new RequestResponse
                {
                    RequestState = "FAIL",
                    RequestMessage = "حدث خطأ برجاء المحاوله لاحقا",
                    Response = null
                });
            }
        }
    }
}
