using System;
using SanitationPortal.Models.DTOs;
using SanitationPortal.Models.Requests;
using SanitationPortal.Models.Response;

namespace SanitationPortal.Service.Services.Interfaces
{
	public interface IAccountServices
	{
		public Task<Response<List<Account>>> GetAccounts();

		public Task<Response<bool>> InsertAccount(Account account);

		public Task<Response<bool>> UpdateAccount(Account account);

		public Task<Response<bool>> UserExists(int employeeId);

		public Task<Response<bool>> RegisterAccount(UserRegisterRequest request);

        public Task<Response<string>> Login(UserLoginRequest request);


    }
}

