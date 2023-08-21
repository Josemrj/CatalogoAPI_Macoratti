using CatalogoAPI.Models;
using CatalogoAPI.Services;
using Microsoft.AspNetCore.Authorization;
using System.Runtime.CompilerServices;

namespace CatalogoAPI.ApiEndpoints
{
    public static class AutenticacaoEndpoints
    {
        public static void MapAutenticacaoEndpoints(this WebApplication app)
        {
            //Login route

            app.MapPost("/login", [AllowAnonymous] (UserModel model, ITokenService tokenService) =>
            {
                if (model == null)
                    return Results.BadRequest("Login Inválido");

                if (model.UserName == "macoratti" && model.Password == "numsey#123")
                {
                    var tokenString = tokenService.GetToken(app.Configuration["Jwt:Key"],
                        app.Configuration["Jwt:Issuer"],
                        app.Configuration["Jwt:Audience"],
                        model);
                    return Results.Ok(new { token = tokenString });
                }
                else
                {
                    return Results.BadRequest("Login Inválido");
                }
            }).Produces(StatusCodes.Status400BadRequest)
              .Produces(StatusCodes.Status200OK)
              .WithName("Login")
              .WithTags("Autenticacao");
        }
    }
}
