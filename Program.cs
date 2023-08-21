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

builder.AddApiSwagger();
builder.AddPersistence();
builder.Services.AddCors();
builder.AddAutenticationJwt();

var app = builder.Build();

app.MapAutenticacaoEndpoints();
app.MapCategoriasEndpoints();
app.MapProdutosEndpoints();
app.MapGetStringForJWTEndpoint();


var enviroment = app.Environment;
app.UseExceptionHandling(enviroment)
   .UseSwaggerMiddleware()
   .UseAppCors();

app.UseAuthentication();
app.UseAuthorization();

app.Run();