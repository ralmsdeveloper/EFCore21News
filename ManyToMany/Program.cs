using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManyToMany
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new ExemploContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var livro = new[]
                {
                    new Livro { Titulo = "EF Core 2.0" },
                    new Livro { Titulo = "EF Core 2.1" }
                };

                var categoria = new[]
                {
                    new Categoria { Descricao = "Versão Preview" },
                    new Categoria { Descricao = "Microsoft" },
                };

                db.AddRange
                (
                    new LivroCategoria { Livro = livro[0], Categoria = categoria[0] },
                    new LivroCategoria { Livro = livro[0], Categoria = categoria[1] },
                    new LivroCategoria { Livro = livro[1], Categoria = categoria[0] },
                    new LivroCategoria { Livro = livro[1], Categoria = categoria[1] }
                );

                db.SaveChanges();

                var livros = db
                    .Livros
                    .Include(e => e.LivrosCategorias)
                    .ThenInclude(e => e.Categoria)
                    .ToList();

                foreach (var l in livros)
                {
                    Console.WriteLine($"Livro: {l.Titulo}");

                    foreach (var c in l.LivrosCategorias.Select(e => e.Categoria))
                    {
                        Console.WriteLine($"\t {c.Descricao}");
                    }
                }

            }

            Console.ReadKey();
        }
    }

    class ExemploContext : DbContext
    {
        public DbSet<Livro> Livros { get; set; }
        public DbSet<Categoria> Categorias { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\Sql2016;Integrated Security=true;Initial Catalog=ManyToMany");

            optionsBuilder.UseLoggerFactory(new LoggerFactory().AddConsole());
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<LivroCategoria>()
                .HasKey(bc => new { bc.LivroId, bc.CategoriaId });
        }
    }

    public class Livro
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public IEnumerable<LivroCategoria> LivrosCategorias { get; set; }
    }

    public class Categoria
    {
        public int CategoriaId { get; set; }
        public string Descricao { get; set; }
        public IEnumerable<LivroCategoria> LivrosCategorias { get; set; }
    }

    public class LivroCategoria
    {
        public int LivroId { get; set; }
        public Livro Livro { get; set; }

        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }
    }
}
