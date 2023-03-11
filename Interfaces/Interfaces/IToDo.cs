using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using Interfaces.Base;
using Interfaces.Helper;
using Interfaces.Helper.ResponseApi;
using Interfaces.ViewModels.ToDoVM;

namespace Interfaces.Interfaces
{
    public interface IToDo : IService
    {
        Task<RequestResponse> GetAll(UserParam param);
        Task<RequestResponse> Get(int id);
        Task<RequestResponse> Create(SaveToDoViewModel model);
        Task<RequestResponse> Update(SaveToDoViewModel model);
        Task<RequestResponse> Delete(int id);
        Task<bool> CheckExist(int id);
        Task<ToDo> GetToDoEntity(int id);
    }
}
