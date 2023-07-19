using Authentication_Authorization_1._0.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Authentication_Authorization_1._0.Helpers;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json")
    .Build();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// Add DbContext to the container
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
});

// Configure JWT authentication
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = false,
//            ValidateAudience = false,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]))
//        };
//    });

// Configure authorization policy
//builder.Services.AddAuthorization();

// Configure Swagger


var app = builder.Build();



app.UseSwagger();
app.UseSwaggerUI();


app.MapGet("/secretkey", () =>
{
    var secretKey = SecretKey.GenerateRandomSecretKey(32);
    return Results.Ok(secretKey);
});

//app.UseHttpsRedirection();

//app.UseAuthentication();
//app.UseAuthorization();


app.MapControllers();

app.Run();
