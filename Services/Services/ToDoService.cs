using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using FluentValidation;
using Interfaces.Helper;
using Interfaces.Helper.ResponseApi;
using Interfaces.Helpers;
using Interfaces.Interfaces;
using Interfaces.ViewModels.ToDoVM;
using Interfaces.ViewModels.UserVM;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Services.Base;
using Services.MapProfile;

namespace Services.Services
{
    public class ToDoService : BaseService, IToDo
    {
        private DataContext _context;
        private IValidator<SaveToDoViewModel> _createTableValidator;
        private ICoreBase _repoCore;
        public ToDoService(DataContext context,
            IValidator<SaveToDoViewModel> createTableValidator,
            ICoreBase repoCore)
        {
            _context = context;
            _createTableValidator = createTableValidator;
            _repoCore = repoCore;
        }

        public async Task<bool> CheckExist(int id)
        {
            return await _context.ToDos.AnyAsync(x => x.Id == id);
        }

        public async Task<RequestResponse> Create(SaveToDoViewModel model)
        {
            var validationResult = _createTableValidator.Validate(model);

            if (!validationResult.IsValid)
            {
                return new RequestResponse { RequestState = "FAIL", RequestMessage = null, Response = new { errorMessages = validationResult.Errors.Select(x => x.ErrorMessage) } };
            }

            var toDo = ObjectMapper.Mapper.Map<ToDo>(model);
            toDo.CreatedAt = DateTime.Now;

            _repoCore.Add(toDo);

            var result = await _repoCore.SaveAll();
            if (!result)
                return new RequestResponse { RequestState = "FAIL", RequestMessage = "Proccess failed, try again", Response = null };

            return new RequestResponse
            {
                RequestState = "PASS",
                RequestMessage = "Process successfull",
                Response = null
            };
        }

        public async Task<RequestResponse> Delete(int id)
        {
            if (!await CheckExist(id))
                return new RequestResponse { RequestState = "FAIL", RequestMessage = "There is no result like this", Response = null };

            _repoCore.Delete(await GetToDoEntity(id));
            var result = await _repoCore.SaveAll();

            if (!result)
                return new RequestResponse { RequestState = "FAIL", RequestMessage = "Proccess failed", Response = null };

            return new RequestResponse
            {
                RequestState = "PASS",
                RequestMessage = "Process successfull",
                Response = null
            };
        }

        public async Task<RequestResponse> Get(int id)
        {
            if (!await CheckExist(id))
                return new RequestResponse { RequestState = "FAIL", RequestMessage = "There is no result like this", Response = null };

            var toDo = await GetToDoEntity(id);

            var model = ObjectMapper.Mapper.Map<GetToDoViewModel>(toDo);

            return new RequestResponse
            {
                RequestState = "PASS",
                RequestMessage = "Get date successfull",
                Response = toDo
            };
        }

        public async Task<RequestResponse> GetAll(UserParam param)
        {
            var toDos = await _context.ToDos.ToListAsync();

            if (!string.IsNullOrEmpty(param.OrderBy))
            if (param.OrderBy.ToLower() == "title")
                toDos = toDos.OrderBy(x => x.Title).ToList();
            else
                toDos = toDos.OrderBy(x => x.CreatedAt).ToList();

            if (!string.IsNullOrEmpty(param.FilterBy))
                toDos = toDos.Where(x => x.Title.Contains(param.FilterBy.ToLower())).ToList();

            var model = ObjectMapper.Mapper.Map<List<GetToDoViewModel>>(toDos);

            return new RequestResponse
            {
                RequestState = "PASS",
                RequestMessage = "Get date successfull",
                Response = new
                {
                    totalPages = PagedList<GetToDoViewModel>.GetTotalPages(model, param.PageSize),
                    data = PagedList<GetToDoViewModel>.CreateAsync(model, param.PageNumber, param.PageSize).Result
                }
            };
        }

        public async Task<ToDo> GetToDoEntity(int id)
        {
            return await _context.ToDos.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<RequestResponse> Update(SaveToDoViewModel model)
        {
            if (!await CheckExist(model.Id))
                return new RequestResponse { RequestState = "FAIL", RequestMessage = "There is no result like this", Response = null };

            var validationResult = _createTableValidator.Validate(model);

            if (!validationResult.IsValid)
            {
                return new RequestResponse { RequestState = "FAIL", RequestMessage = null, Response = new { errorMessages = validationResult.Errors.Select(x => x.ErrorMessage) } };
            }

            var toDo = ObjectMapper.Mapper.Map(model, await GetToDoEntity(model.Id));
            toDo.UpdatedAt = DateTime.Now;

            await _repoCore.SaveAll();

            return new RequestResponse
            {
                RequestState = "PASS",
                RequestMessage = "Process successfull",
                Response = null
            };
        }
    }
}
