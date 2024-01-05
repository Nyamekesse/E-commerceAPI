using E_commerceAPI;
using E_commerceAPI.Data;
using E_commerceAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbUserName = Environment.GetEnvironmentVariable("DB_USERNAME");

if (string.IsNullOrEmpty(dbName) || string.IsNullOrEmpty(dbHost) || string.IsNullOrEmpty(dbPassword) || string.IsNullOrEmpty(dbUserName) || string.IsNullOrEmpty(Environment.GetEnvironmentVariable("MAIL_GUN_API_KEY")) || string.IsNullOrEmpty(Environment.GetEnvironmentVariable("MAILGUN_DOMAIN")))
{
    Console.WriteLine("Provide all values for environment variables");
    return;
}

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<EmailSender>();
builder.Services.AddAutoMapper(typeof(MappinConfig));
builder.Services.AddDbContext<ApplicationDBContext>(option => { option.UseNpgsql($"Host={dbHost}; Database={dbName}; Username={dbUserName}; Password={dbPassword}"); });

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
