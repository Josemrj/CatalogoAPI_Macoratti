using CatalogoAPI.Context;
using CatalogoAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoAPI.ApiEndpoints
{
    public static class ProdutosEndpoints
    {
        public static void MapProdutosEndpoints(this WebApplication app)
        {
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
                try
                {
                    if (produto == null)
                        return Results.NotFound("RECURSO NAO ENCONTRADO!");

                    await db.AddAsync(produto);

                    db.SaveChanges();

                    return Results.Created($"/produtos/{produto.ProdutoId}:int", produto);
                }
                catch (Exception)
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
        }
    }
}
