using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using QROrdering.API.Middleware;
using QROrdering.Application.Authentication;
using QROrdering.Application.Authentication.Interfaces;
using QROrdering.Application.Common.Interfaces;
using QROrdering.Infrastructure.Authentication;
using QROrdering.Infrastructure.Configurations;
using QROrdering.Infrastructure.Persistence;
using QROrdering.Infrastructure.Redis;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// =========================
// Configuration
// =========================

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

builder.Services.Configure<CacheSettings>(
    builder.Configuration.GetSection("CacheSettings"));

var jwtSettings = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtSettings>()
    ?? throw new Exception("Jwt configuration is missing.");


// =========================
// DbContext
// =========================

builder.Services.AddDbContext<QROrderingDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(
        builder.Configuration["Redis:ConnectionString"]
        ?? throw new Exception("Redis configuration is missing.")
    ));

builder.Services.AddMemoryCache();

builder.Services.AddSingleton<RedisService>();


// =========================
// Authentication Services
// =========================

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserSessionRepository, UserSessionRepository>();

builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IHashService, HashService>();
builder.Services.AddScoped<IJwtBlacklistService, JwtBlacklistService>();
builder.Services.AddScoped<ISessionCacheService, SessionCacheService>();

builder.Services.AddScoped<JwtAuthEvents>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


// =========================
// Current User / Request Info
// =========================

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<
    ICurrentUserService,
    CurrentUserService>();

builder.Services.AddScoped<
    IRequestInfoService,
    RequestInfoService>();


// =========================
// JWT Authentication
// =========================
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwtSettings.Key)),

                RoleClaimType = ClaimTypes.Role,
                NameClaimType = "username"
            };

        // Giữ nguyên custom claims:
        // userId, username, sid, user_type...
        options.MapInboundClaims = false;

        // JwtAuthEvents được resolve bởi DI
        options.EventsType = typeof(JwtAuthEvents);
    });

// =========================
// Authorization
// =========================

builder.Services.AddAuthorization();


// =========================
// Controllers
// =========================

builder.Services.AddControllers();


// =========================
// Swagger / OpenAPI
// =========================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    // API information
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "QROrdering API",
        Version = "v1",
        Description = "QR Ordering SaaS API"
    });

    // JWT Access Token
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,

        Description =
            "JWT Access Token\n\n" +
            "Format: Bearer {your_access_token}\n\n" +
            "Refresh Token is saved by HttpOnly Cookie."
    });

    // Áp dụng JWT Security cho Swagger operations
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Tránh duplicate schema name
    c.CustomSchemaIds(type => type.FullName);
});

var app = builder.Build();


// =========================
// Global Exception Middleware
// =========================

app.UseMiddleware<GlobalExceptionMiddleware>();


// =========================
// Response Time Middleware
// =========================

app.UseMiddleware<ResponseTimeMiddleware>();


// =========================
// Swagger
// =========================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        // Giữ JWT khi refresh Swagger
        c.ConfigObject.PersistAuthorization = true;

        c.DocumentTitle = "QROrdering API Docs";
    });
}


// =========================
// HTTPS
// =========================

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseMiddleware<SessionValidationMiddleware>();

app.UseAuthorization();

app.MapControllers();


// =========================
// Run Application
// =========================

app.Run();