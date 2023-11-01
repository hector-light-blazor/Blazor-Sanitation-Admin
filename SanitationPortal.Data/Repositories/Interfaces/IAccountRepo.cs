using System;
using SanitationPortal.Data.Entities;
namespace SanitationPortal.Data.Repositories.Interfaces
{
	public interface IAccountRepo
	{
		public Task<Account> GetAccount(int Id);

        public Task<List<Account>> GetAccounts();

		public Task<bool> InsertAccount(Account account);

		public Task<bool> UpdateAccount(Account account);

		public Task<bool> UserExists(int employeeId);

		public Task<bool> EmailExists(string email);

		public Task<Account> Login(int employeeId, string password);

    }
}

