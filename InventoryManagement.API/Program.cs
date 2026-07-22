using InventoryManagement.API.Configurtion;
using InventoryManagement.API.Data;
using InventoryManagement.API.Repositories.Implementations;
using InventoryManagement.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IProductRepository,ProductRepository>();
builder.Services.AddScoped<IUserRepository,UserRepository>();
builder.Services.AddScoped<IStockTransactionRepository,StockTransactionRepository>();

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt")
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

