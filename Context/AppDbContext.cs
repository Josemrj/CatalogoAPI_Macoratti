using CatalogoAPI.Models;
using Microsoft.EntityFrameworkCore;
namespace CatalogoAPI.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Produto> Produtos { get; set; }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            //categorias
            mb.Entity<Categoria>().HasKey(c => c.CategoriaId);
            mb.Entity<Categoria>().Property(c => c.Nome)
                                  .HasMaxLength(100)
                                  .IsRequired();

            
            //produto
            mb.Entity<Produto>().HasKey(c => c.ProdutoId);
            mb.Entity<Produto>().Property(c => c.Nome)
                                .HasMaxLength(100)
                                .IsRequired();
            mb.Entity<Produto>().Property(c => c.Descricao).HasMaxLength(100);
            mb.Entity<Produto>().Property(c => c.Descricao).HasMaxLength(100);
            mb.Entity<Produto>().Property(c => c.Imagem).HasMaxLength(100);
            mb.Entity<Produto>().Property(c => c.Preco).HasPrecision(14, 2);

            
            //relacionamento
            mb.Entity<Produto>().HasOne<Categoria>(c => c.Categoria)
                                .WithMany(p => p.Produto)
                                .HasForeignKey(c => c.CategoriaId);
        }
    
    }
}
