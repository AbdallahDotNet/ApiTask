using Interfaces.Helper;
using Interfaces.Helper.ResponseApi;
using Interfaces.Helpers;
using Interfaces.Interfaces;
using Interfaces.ViewModels.ToDoVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace ToDoListApp.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class ToDosController : ControllerBase
    {
        public static NLog.ILogger logger = LogManager.GetCurrentClassLogger();
        private IToDo _repo;
        public ToDosController(IToDo repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery]UserParam param)
        {
            try
            {
                return Ok(await _repo.GetAll(param));
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError(ex, logger, "[/api/v1/todos]");

                return BadRequest(new RequestResponse
                {
                    RequestState = "FAIL",
                    RequestMessage = "حدث خطأ برجاء المحاوله لاحقا",
                    Response = null
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                return Ok(await _repo.Get(id));
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError(ex, logger, "[/api/v1/todos/{id}]");

                return BadRequest(new RequestResponse
                {
                    RequestState = "FAIL",
                    RequestMessage = "حدث خطأ برجاء المحاوله لاحقا",
                    Response = null
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaveToDoViewModel model)
        {
            try
            {
                return Ok(await _repo.Create(model));
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError(ex, logger, "[/api/v1/todos/create]");

                return BadRequest(new RequestResponse
                {
                    RequestState = "FAIL",
                    RequestMessage = "حدث خطأ برجاء المحاوله لاحقا",
                    Response = null
                });
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(SaveToDoViewModel model)
        {
            try
            {
                return Ok(await _repo.Update(model));
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError(ex, logger, "[/api/v1/todos/update]");

                return BadRequest(new RequestResponse
                {
                    RequestState = "FAIL",
                    RequestMessage = "حدث خطأ برجاء المحاوله لاحقا",
                    Response = null
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                return Ok(await _repo.Delete(id));
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError(ex, logger, "[/api/v1/todos/delete]");

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
