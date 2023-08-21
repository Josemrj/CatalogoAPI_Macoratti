using CatalogoAPI.ApiEndpoints;
using CatalogoAPI.AppServicesExtensions;
using CatalogoAPI.Context;
using CatalogoAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

string stringConnection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(opt =>
                 opt.UseMySql(stringConnection, ServerVersion
                .AutoDetect(stringConnection)));

//-----------Add Services for JWT-----------------------------
builder.Services.AddSingleton<ITokenService> (new TokenService());

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
//-----------Add Middleware for JWT-----------------------------
builder.Services.AddAuthorization();


var app = builder.Build();

app.MapAutenticacaoEndpoints();
app.MapCategoriasEndpoints();
app.MapProdutosEndpoints();
app.MapGetStringForJWTEndpoint();


var enviroment = app.Environment;

app.UseExceptionHandling(enviroment)
   .UseSwaggerMiddleware()
   .UseAppCors();

//Use midlleware for JWT
app.UseAuthentication();
app.UseAuthorization();

app.Run();