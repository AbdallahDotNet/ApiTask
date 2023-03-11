using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using Interfaces.Base;
using Interfaces.Helper.ResponseApi;
using Interfaces.ViewModels.UserVM;

namespace Interfaces.Interfaces
{
    public interface IUser : IService
    {
        JwtSecurityToken GenerateToken(Entities.Models.User user);
        Task<RequestResponse> Login(LoginViewModel model);
        Task<RequestResponse> CreateUser(SaveUserViewModel model);
        Task<RequestResponse> UpdateUser(SaveUserViewModel model);
        Task<bool> CheckExist(string email);
        Task<bool> CheckExist(int id);
        Task<Entities.Models.User> GetUser(string email);
        Task<Entities.Models.User> GetUser(int id);
        Task<bool> CheckPasswordIsCorrect(string plainPassword, Entities.Models.User user);
    }
}
