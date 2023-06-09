using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using SanitationPortal.Areas.Identity;
using SanitationPortal.Data;
using SanitationPortal.Data.Extension;
using Microsoft.Extensions.Hosting;
using MudBlazor.Services;
using SanitationPortal.Service.Services.Interfaces;
using SanitationPortal.Service.Services;
using SanitationPortal.Data.Repositories.Interfaces;
using SanitationPortal.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");


builder.Services.AddDbContextFactory<SanitationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

//Add hot reload
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

builder.Services.AddServerSideBlazor();

builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();

//Add Mud Services
builder.Services.AddMudServices();

builder.Services.AddSingleton<IAccountRepo, AccountRepo>();
builder.Services.AddSingleton<IAccountServices, AccountService>();

var app = builder.Build();

//Apply Migrations for Identity and Sanitation Porta
app.ApplyMigrations<ApplicationDbContext>()
   .ApplyMigrations<SanitationDbContext>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");



app.Run();

