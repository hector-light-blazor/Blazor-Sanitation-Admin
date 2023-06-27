using System;
using Microsoft.EntityFrameworkCore;
using SanitationPortal.Data.Entities;
using SanitationPortal.Data.Repositories.Interfaces;


namespace SanitationPortal.Data.Repositories
{
	public class AccountRepo : IAccountRepo
	{
		private readonly IDbContextFactory<SanitationDbContext> _factory;

        public AccountRepo(IDbContextFactory<SanitationDbContext> factory)
		{
			_factory = factory;
		}


		public async Task<Account> GetAccount(int Id)
		{
			using var context = await _factory.CreateDbContextAsync();

			var account = await context.Accounts.FirstOrDefaultAsync(a => a.Id == Id);
			if (account != null)
				return account;

			return new();
		}

		public async Task<List<Account>> GetAccounts()
		{
			using var context = _factory.CreateDbContext();

			return await context.Accounts.ToListAsync();
		}

		public async Task<bool> InsertAccount(Account account)
		{
			using var context = await _factory.CreateDbContextAsync();

			var response = await context.Accounts.AddAsync(account);

			await context.SaveChangesAsync();

			bool isSuccess = account.Id > 0;


            return isSuccess;
		}

        public async Task<Account> Login(int employeeId, string password)
        {
			using var context = await _factory.CreateDbContextAsync();
			var account = await context.Accounts
							.FirstOrDefaultAsync(account => 
							account.EmployeeId == employeeId && account.DigestPassword.Equals(password));
			return account ?? new Account();
        }

        public async Task<bool> UpdateAccount(Account account)
		{
			using var context = await _factory.CreateDbContextAsync();

			var accountDb = await context.Accounts.FirstOrDefaultAsync(a => a.Id == account.Id);

			if (accountDb != null)
			{
				context.Entry<Account>(accountDb).CurrentValues.SetValues(account);

				context.Accounts.Update(accountDb);

				var result = await context.SaveChangesAsync();

				return result > 0;
			}

			return false;
		}

        public async Task<bool> UserExists(int employeeId)
        {
            using var context = await _factory.CreateDbContextAsync();

			return await context.Accounts.AnyAsync(account => account.Id == employeeId);
        }
    }
}

