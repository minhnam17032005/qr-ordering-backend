using Microsoft.EntityFrameworkCore;
using QROrdering.API.Middleware;
using QROrdering.Application.Authentication;
using QROrdering.Application.Authentication.Interfaces;
using QROrdering.Infrastructure.Authentication;
using QROrdering.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// =========================
// DbContext
// =========================
builder.Services.AddDbContext<QROrderingDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// =========================
// Authentication Services
// =========================
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordService, PasswordService>();

// =========================
// Controllers
// =========================
builder.Services.AddControllers();

// =========================
// Swagger / OpenAPI
// =========================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// =========================
// Global Exception Middleware
// =========================
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<ResponseTimeMiddleware>();

// =========================
// Swagger
// =========================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// =========================
// HTTPS
// =========================
app.UseHttpsRedirection();

// =========================
// Controllers
// =========================
app.MapControllers();

app.Run();