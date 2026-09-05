using Microsoft.EntityFrameworkCore;
using QROrdering.API.Middleware;
using QROrdering.Application.Authentication;
using QROrdering.Application.Authentication.Interfaces;
using QROrdering.Application.Common.Interfaces;
using QROrdering.Infrastructure.Authentication;
using QROrdering.Infrastructure.Configurations;
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
builder.Services.AddScoped<IUserSessionRepository, UserSessionRepository>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IHashService, HashService>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<
    IRequestInfoService,
    RequestInfoService>();
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