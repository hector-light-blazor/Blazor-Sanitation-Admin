using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SanitationPortal.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }


    public static void SeedDefaultUser(UserManager<IdentityUser> userManager)
    {
        if (userManager.FindByNameAsync("admin").Result == null)
        {
            var user = new IdentityUser
            {
                UserName = "admin@mcallen.net",
                Email = "roensebastian2015@gmail.com",
                EmailConfirmed = true,
            };

            var result = userManager.CreateAsync(user, "Mcallen2023!").Result;

            if (result.Succeeded)
            {
                // You can add roles, claims, or other custom properties here.
            }
        }
    }
}

