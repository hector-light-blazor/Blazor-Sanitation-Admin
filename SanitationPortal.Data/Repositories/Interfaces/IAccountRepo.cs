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

    }
}

