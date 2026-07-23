using System.Text;
using InventoryManagement.API.Configurtion;
using InventoryManagement.API.Data;
using InventoryManagement.API.Repositories.Implementations;
using InventoryManagement.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var jwtSettings=builder.Configuration
                                    .GetSection("Jwt")
                                    .Get<JwtSettings>()!;

                    options.TokenValidationParameters=
                            new TokenValidationParameters
                            {
                                ValidateIssuer=true,
                                ValidateAudience=true,
                                ValidateLifetime=true,
                                ValidateIssuerSigningKey=true,

                                ValidIssuer=jwtSettings.Issuer,
                                ValidAudience=jwtSettings.Audience,

                                IssuerSigningKey=new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(jwtSettings.Key))  
                            };
                });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.Run();

