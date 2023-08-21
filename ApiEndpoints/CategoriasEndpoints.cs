using CatalogoAPI.Context;
using CatalogoAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoAPI.ApiEndpoints;

public static class CategoriasEndpoints
{
    public static void MapCategoriasEndpoints(this WebApplication app)
    {
        app.MapGet("/categorias", async (AppDbContext db) => await db.Categorias.ToListAsync()).WithTags("Categorias").RequireAuthorization();

        app.MapGet("/categorias/{id:int}", async (int id, AppDbContext db) =>
        {
            var categoria = await db.Categorias.FindAsync(id);

            if (categoria == null)
                return Results.NotFound();

            return Results.Ok(categoria);
        });

        app.MapGet("/categorias-new/{id:int}", async (int id, AppDbContext db) => await db.Categorias.FindAsync(id) is Categoria categoria ? Results.Ok(categoria) : Results.NotFound("RECURSO NAO ENCONTRADO"));

        app.MapPost("/categorias", async (Categoria categoria, AppDbContext db) =>
        {
            await db.AddAsync(categoria);
            db.SaveChanges();

            return Results.Created($"/categoria/{categoria.CategoriaId}:int", categoria);
        });

        app.MapPut("/categorias", async (int id, Categoria categoria, AppDbContext db) =>
        {
            if (categoria.CategoriaId != id)
                return Results.NotFound();

            var categorias = await db.Categorias.FindAsync(id);

            if (categorias == null)
                return Results.NotFound();

            categorias.Nome = categoria.Nome;
            categorias.Descricao = categoria.Descricao;

            await db.SaveChangesAsync();

            return Results.Accepted($"/categorias-new{categorias.CategoriaId}:int", categorias);
        });

        app.MapDelete("/categorias/{id:int}", async (int id, AppDbContext db) =>
        {
            var categorias = db.Categorias.FirstOrDefault(a => a.CategoriaId == id);

            if (categorias == null)
                return Results.NotFound("RECURSO NAO ENCONTRADO");

            db.Remove(categorias);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}

