using System;
using SanitationPortal.Models.DTOs;

namespace SanitationPortal.Service.Services.Interfaces
{
	public interface IAccountServices
	{
		public Task<List<Account>> GetAccounts();

		public Task<bool> InsertAccount(Account account);

		public Task<bool> UpdateAccount(Account account);

    }
}

