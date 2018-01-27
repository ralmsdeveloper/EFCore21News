using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace TiposDerivados20
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new ExemploContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                var query = db.Familias.Include("Filho").ToList();
            }
        }
    }

    class ExemploContext : DbContext
    {
        public DbSet<Familia> Familias { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\Sql2016;Integrated Security=true;Initial Catalog=TiposDerivados20");
            //Adicionar Log
            optionsBuilder.UseLoggerFactory(new LoggerFactory().AddConsole());
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Familia>().HasKey(p => p.Id);

            modelBuilder
                .Entity<Pais>()
                .HasBaseType<Familia>()
                .HasMany(p => p.Filho);

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
