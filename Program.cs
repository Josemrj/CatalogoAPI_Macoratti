using CatalogoAPI.ApiEndpoints;
using CatalogoAPI.Context;
using CatalogoAPI.Models;
using CatalogoAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Diagnostics.Metrics;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
//Swagger modifield
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CatalogoAPI", Version = "v1.1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.\r\n\r\n Enter 'Bearer'[space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
    });
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
                         new string[] {}
                    }
                });
});

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

//app.MapGet("/", async () => "Welcome to application!").ExcludeFromDescription();
app.MapCategoriasEndpoints();
//-------------------------[Product]---------------------------------------------------------------------


app.MapGet("/produtos", async (AppDbContext db) => await db.Produtos.ToListAsync() is null ? Results.NotFound("REC. NAO ENCONTRADO") : Results.Ok(await db.Produtos.ToListAsync())).WithTags("Produtos").RequireAuthorization();

app.MapGet("/produtos/{id:int}", async (int id, AppDbContext db) =>
{
    var products = await db.Produtos.FindAsync(id);

    if (products.ProdutoId != id)
        return Results.BadRequest("REQUISICAO INVALIDA");

    if (products == null)
        return Results.NotFound("RECURSO NAO ENCONTRADO");

    return Results.Ok(products);
});

app.MapPost("/produtos", async (Produto produto, AppDbContext db) =>
{
    try { 
    if (produto == null)
        return Results.NotFound("RECURSO NAO ENCONTRADO!");
    
    await db.AddAsync(produto);

    db.SaveChanges();

    return Results.Created($"/produtos/{produto.ProdutoId}:int", produto);
    }
    catch(Exception)
    {
        return Results.StatusCode(StatusCodes.Status500InternalServerError);
    }
});

app.MapPut("/produtos/{id:int}", async (int id, Produto inputProduto, AppDbContext db) =>
{
    if (inputProduto == null)
        return Results.NotFound();
    
    var produto = await db.Produtos.FindAsync(id);

    if (inputProduto.ProdutoId != id)
        return Results.BadRequest("REQUISICAO INVALIDA");

    produto.Nome = inputProduto.Nome;
    produto.Descricao = inputProduto.Descricao;
    produto.Preco = inputProduto.Preco;
    produto.Imagem = inputProduto.Imagem;
    produto.DataCompra = inputProduto.DataCompra;
    produto.Estoque = inputProduto.Estoque;

    await db.SaveChangesAsync();

    return Results.Accepted($"/produtos/{produto.ProdutoId}:int", produto);
});

app.MapDelete("/produtos/{id:int}", async (int id, AppDbContext db) =>
{
     var produtos = await db.Produtos.FindAsync(id);

    if (produtos == null)
        return Results.NotFound();

    db.Remove(produtos);

    await db.SaveChangesAsync();

    return Results.NoContent();
});

//---------------------------GET KEY--------------------------

app.MapGet("/generate-kay", async () =>
{
    string getString = "";
    GenerateKay();

    ActionResult<string> GenerateKay()
    {
        int keyLength = 32; // Tamanho da chave em bytes (256 bits)
        byte[] keyBytes = new byte[keyLength];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(keyBytes);
        }
        string jwtSecretKey = Convert.ToBase64String(keyBytes);
        getString = jwtSecretKey;

        return jwtSecretKey;
    }

    return Results.Ok(getString);
});


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Use midlleware for JWT
app.UseAuthentication();
app.UseAuthorization();

app.Run();