using Microsoft.EntityFrameworkCore;

using SanitationPortal.Data.Entities;

namespace SanitationPortal.Data;
public class SanitationDbContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }

    public SanitationDbContext(DbContextOptions<SanitationDbContext> options) : base(options)
    {

    }
}

