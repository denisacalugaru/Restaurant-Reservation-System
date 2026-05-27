using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RestaurantReservation.API.Data;
using RestaurantReservation.API.Models;
using RestaurantReservation.API.Repositories;
using RestaurantReservation.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFsegdsHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

var jwtKey = builder.Configuration["Jwt:Key"] ?? "abcdefghijklmnopqrstuvwxyzABCDEFsegdsHIJKLMNOPQRSTUVWXYZ0123456789-._@+\"";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "RestaurantAPI",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "RestaurantUI",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularCors", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200", "http://localhost:4201", "http://localhost:51975")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddScoped<IRestaurantRepository, RestaurantRepository>();
builder.Services.AddScoped<IRestaurantService, RestaurantService>();

builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IReservationService, ReservationService>();
var app = builder.Build();

app.UseCors("AngularCors");

app.UseMiddleware<RestaurantReservation.API.Middleware.ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();