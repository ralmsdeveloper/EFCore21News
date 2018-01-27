using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace TiposDerivados
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new ExemploContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                db.Familias.Add(new Pais
                {
                    Nome = "Familia Teste",
                    NomeMae = "Lourdes",
                    NomePai = "Reginaldo",
                    Filho = new[]
                    {
                        new Filho
                        {
                             NomeFilho = "RAFAEL"
                        }
                        ,
                        new Filho
                        {
                             NomeFilho = "FABIO"
                        }
                    }
                });
                db.SaveChanges();

                var query = db
                    .Familias
                    .AsNoTracking()
                    .Include("Filho")
                    .ToList();

                WriteLine($"Posição [0]: {query[0] is Pais}");
                WriteLine($"Posição [0]: {query[0] is Filho}");
                WriteLine($"Posição [0]: {query[0] is Filho}");
                WriteLine($"Posição [1]: {query[1] is Filho}");

                ReadKey();
            }
        }
    }

    class ExemploContext : DbContext
    {
        public DbSet<Familia> Familias { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\Sql2016;Integrated Security=true;Initial Catalog=TiposDerivados");

            optionsBuilder.UseLoggerFactory(new LoggerFactory().AddConsole());
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Familia>().HasKey(p => p.Id);

            modelBuilder
                .Entity<Pais>()
                .HasBaseType<Familia>()
                .HasMany(f => f.Filho);

            base.OnModelCreating(modelBuilder);
        }
    }

    public class Familia
    {
        public int Id { get; set; }
        public string Nome { get; set; }
    }

    public class Pais : Familia
    {
        public string NomePai { get; set; }
        public string NomeMae { get; set; }

        public IEnumerable<Filho> Filho { get; set; }
    }

    public class Filho : Pais
    {
        public string NomeFilho { get; set; }
    }
}
