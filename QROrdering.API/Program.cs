using Microsoft.EntityFrameworkCore;
using QROrdering.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// =========================
// DbContext
// =========================
builder.Services.AddDbContext<QROrderingDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

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
// Middleware
// =========================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();