using System;
using SanitationPortal.Service.Services.Interfaces;
using SanitationPortal.Data.Repositories.Interfaces;
using SanitationPortal.Models.DTOs;
using SanitationPortal.Models.Extensions;
using SanitationPortal.Models.Response;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using SanitationPortal.Models.Requests;
using Microsoft.Extensions.Configuration;

namespace SanitationPortal.Service.Services
{
	public class AccountService : IAccountServices
    {
		private readonly IAccountRepo _accountRepo;
		private readonly IConfiguration _configuration;

        public AccountService(IAccountRepo accountRepo, IConfiguration configuration)
		{
			_accountRepo = accountRepo;
			_configuration = configuration;
		}
		public async Task<Response<List<Account>>> GetAccounts()
		{
			var response = new Response<List<Account>>();
			response.Success = true;

			var accountEntity = await _accountRepo.GetAccounts();
			response.Success = accountEntity.Any();

			response.Data = (response.Success) ? accountEntity.Select(a => a.ToModel()).ToList() :
				new List<Account>();


            return response;
		}

		public async Task<Response<bool>> InsertAccount(Account account)
		{
			var response = new Response<bool>();
			response.Success = false;

			var userExists = UserExists(account.EmployeeId).GetAwaiter().GetResult();
			if(userExists.Success is false) 
			{			
				var entity = account.ToEntity();

				response.Success = await _accountRepo.InsertAccount(entity);
				return response;
			}

			response.Errors = new List<Error>() { new Error { ErrorCode = 500, Message = "User Exists" } };

			return response;
		}

        public async Task<Response<string>> Login(UserLoginRequest request)
        {
			var response = new Response<string>();
			response.Success = false;
			var entity = request.ToEntity();
			var account = await _accountRepo.Login(entity.EmployeeId, entity.DigestPassword);
			response.Success = account.Id != 0;
			response.Data = (response.Success) ? "Valid Account!" : "Not Valid Account";

			response.Token = (response.Success) ? CreateToken(account.ToModel()) : null;

			return response;
        }

        public async Task<Response<bool>> RegisterAccount(UserRegisterRequest request)
        {
            var response = new Response<bool>();
            response.Success = false;

            var userExists = UserExists(request.EmployeeId).GetAwaiter().GetResult();
            if (userExists.Success is false)
            {
                var entity = request.ToEntity();

                response.Success = await _accountRepo.InsertAccount(entity);
                return response;
            }

			response.Errors = new List<Error>() { new Error { ErrorCode = 500, Message = "Employee Id exists" } };

			return	response;
        }

        public async Task<Response<bool>> UpdateAccount(Account account)
		{
			var response = new Response<bool>();
			response.Success = false;



			var entity = account.ToEntity();
			response.Success = await _accountRepo.UpdateAccount(entity);

			return response;
		}

        public  async Task<Response<bool>> UserExists(int employeeId)
        {
            var response = new Response<bool>();
			response.Success = false;

			try
			{
				response.Success = await _accountRepo.UserExists(employeeId);
			}
			catch (Exception ex)
			{
				response.Errors = new List<Error> {new Error{
						ErrorCode = ex.GetHashCode(),
						Message = ex.Message,
				}};
			}

			return response;
        }

		private string CreateToken(Account account) 
		{
			List<Claim> claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
				new Claim(ClaimTypes.Name, account.Email)
			};

			var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8
				.GetBytes(_configuration?.GetSection("AppSettings:Token")?.Value ?? ""));

			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

			var token = new JwtSecurityToken(
				   claims: claims,
				   expires: DateTime.Now.AddDays(1),
				   signingCredentials: creds
				);

			var jwt = new JwtSecurityTokenHandler().WriteToken(token);

			return jwt;
		}
    }
}

