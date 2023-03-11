using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using FluentValidation;
using Interfaces.Helper.ResponseApi;
using Interfaces.Helpers;
using Interfaces.Interfaces;
using Interfaces.ViewModels.UserVM;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Base;
using Services.MapProfile;

namespace Services.Services
{
    public class UserService : BaseService, IUser
    {
        private DataContext _context;
        private IConfiguration _configuration;
        private IValidator<SaveUserViewModel> _createTableValidator;
        private ICoreBase _repoCore;
        public UserService(DataContext context,
            IConfiguration configuration,
            IValidator<SaveUserViewModel> createTableValidator,
            ICoreBase repoCore)
        {
            _context = context;
            _configuration = configuration;
            _createTableValidator = createTableValidator;
            _repoCore = repoCore;
        }

        public async Task<bool> CheckExist(string email)
        {
            return await _context.Users.AnyAsync(x => x.Email == email);
        }

        public async Task<bool> CheckExist(int id)
        {
            return await _context.Users.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> CheckPasswordIsCorrect(string plainPassword, User user)
        {
            return (HashHelper.Decrypt(user.Password) == plainPassword) ? true : false;
        }

        public async Task<RequestResponse> CreateUser(SaveUserViewModel model)
        {
            var validationResult = _createTableValidator.Validate(model);

            if (!validationResult.IsValid)
            {
                return new RequestResponse { RequestState = "FAIL", RequestMessage = null, Response = new { errorMessages = validationResult.Errors.Select(x => x.ErrorMessage) } };
            }
            
            model.Password = HashHelper.Encrypt(model.Password);

            var user = ObjectMapper.Mapper.Map<User>(model);
            _repoCore.Add(user);

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

        public JwtSecurityToken GenerateToken(User user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var newToken = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddMinutes(30),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

            return newToken;
        }

        public async Task<User> GetUser(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User> GetUser(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<RequestResponse> Login(LoginViewModel model)
        {
            if (string.IsNullOrEmpty(model.Email))
                return new RequestResponse { RequestState = "FAIL", RequestMessage = "Please, enter your email", Response = null };

            if (string.IsNullOrEmpty(model.Password))
                return new RequestResponse { RequestState = "FAIL", RequestMessage = "Please, enter your password", Response = null };

            if (!await CheckExist(model.Email))
                return new RequestResponse { RequestState = "FAIL", RequestMessage = "Please, check your entries", Response = null };

            var user = await GetUser(model.Email);

            if (!await CheckPasswordIsCorrect(model.Password, user))
                return new RequestResponse { RequestState = "FAIL", RequestMessage = "Please, check your entries", Response = null };

            var token = GenerateToken(user);

            return new RequestResponse
            {
                RequestState = "PASS",
                RequestMessage = "Login successfull",
                Response = new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expireDate = token.ValidTo
                }
            };
        }

        public async Task<RequestResponse> UpdateUser(SaveUserViewModel model)
        {
            if (!await CheckExist(model.Id))
                return new RequestResponse { RequestState = "FAIL", RequestMessage = "This user doesn't exist", Response = null };

            var validationResult = _createTableValidator.Validate(model);

            if (!validationResult.IsValid)
            {
                return new RequestResponse { RequestState = "FAIL", RequestMessage = null, Response = new { errorMessages = validationResult.Errors.Select(x => x.ErrorMessage) } };
            }

            model.Password = HashHelper.Encrypt(model.Password);

            var user = ObjectMapper.Mapper.Map(model, await GetUser(model.Id));
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
