using APICatalogo.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Data
{
    public class APICatalogoDbContext : IdentityDbContext
    {
        public APICatalogoDbContext(DbContextOptions<APICatalogoDbContext> options) : base(options)
        {
               
        }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Produto> Produtos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Categoria>(x =>
            {
                x.HasKey(x => x.Id);
                x.Property(x => x.Nome).HasColumnType("VARCHAR(100)").IsRequired();
                x.Property(x => x.ImagemUrl).HasColumnType("VARCHAR(MAX)").IsRequired();
            });

            modelBuilder.Entity<Produto>(x =>
            {
                x.HasKey(x => x.Id);
                x.Property(x => x.Nome).HasColumnType("VARCHAR(100)").IsRequired();
                x.Property(x => x.Descricao).HasColumnType("TEXT").IsRequired();
                x.Property(x => x.Preco).HasColumnType("DECIMAL(8,2)").IsRequired();
                x.Property(x => x.ImagemUrl).HasColumnType("TEXT").IsRequired();
                x.Property(x => x.Estoque).HasColumnType("FLOAT").IsRequired();
                x.Property(x => x.DataCadastro).HasColumnType("DATE").IsRequired(); //EF vai gerar esse codigo SQL na hora da criação do banco
                x.HasOne(p => p.Categoria) //1 produto tem 1 categoria
                    .WithMany(c => c.Produtos) //1 categoria tem muitos produtos (N)
                    .HasForeignKey(p => p.CategoriaId); //Chave estrangeira em produto (lado N)
            });
        }
    }
}
