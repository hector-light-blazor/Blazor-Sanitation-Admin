using System;
using SanitationPortal.Service.Services.Interfaces;
using SanitationPortal.Data.Repositories.Interfaces;
using SanitationPortal.Models.DTOs;
using SanitationPortal.Models.Extensions;

namespace SanitationPortal.Service.Services
{
	public class AccountService : IAccountServices
    {
		private readonly IAccountRepo _accountRepo;

        public AccountService(IAccountRepo accountRepo)
		{
			_accountRepo = accountRepo;
		}
		public async Task<List<Account>> GetAccounts()
		{
			var accountEntity = await _accountRepo.GetAccounts();

			return accountEntity.Select(a => a.ToModel()).ToList();
		}

		public async Task<bool> InsertAccount(Account account)
		{
			var entity = account.ToEntity();

			var response = await _accountRepo.InsertAccount(entity);

			return response;
		}

		public async Task<bool> UpdateAccount(Account account)
		{

			var entity = account.ToEntity();
			
			return await _accountRepo.UpdateAccount(entity);
		}
	}
}

