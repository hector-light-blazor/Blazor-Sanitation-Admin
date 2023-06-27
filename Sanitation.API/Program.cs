using Microsoft.EntityFrameworkCore;
using SanitationPortal.Data;
using SanitationPortal.Data.Repositories;
using SanitationPortal.Data.Repositories.Interfaces;
using SanitationPortal.Service.Services;
using SanitationPortal.Service.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

//Connect to PostgreSQL
builder.Services.AddDbContextFactory<SanitationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add services to the containers
builder.Services.AddSingleton<IAccountRepo, AccountRepo>();
builder.Services.AddSingleton<IAccountServices, AccountService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
